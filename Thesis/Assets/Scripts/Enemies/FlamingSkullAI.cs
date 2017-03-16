using UnityEngine;
using System.Collections;

public class FlamingSkullAI : Enemy {

	Vector3 targetPosition;
	public Transform fireballSpawnLocation;
	public Rigidbody2D jaw;
	public AudioSource chatteringTeeth;
	public ParticleSystem fire;
	public ParticleSystem smoke;
	public Explode explode;
	public Collider2D myCollider;
	bool exploded = false;

	[Header("FlamingSkull Attack")]
	public float attackSpeed = 6f;
	private bool attacking = false;
	public bool firing = false;
	public bool charging = false;
	public float chargeAttackDuration = 1;
	public float riseHeight = 4f;
	public GameObject fireballPrefab;
	public float fireballAttackChance = .3f;
	public float perFireballDelay = 1f;
	public int fireBalls = 3;
	public float fireballSpeed = 10;
	public float fireballChargeTime = 1;
	public float healthRatioLimit = 0.5f; //if the health is under this %, speed up attacks or change slightly
	public float subRatioMultiplier = 2; //multiply or divide times and speeds by this if health under ratio
	float magnitude;
	public float bounceTolerance = 40;

	GameObject currentFireball;

	public override void Awake()
	{
		base.Awake();
		myCollider.enabled = false;
	}

	public override void FoundTarget(Entity e)
	{
		base.FoundTarget(e);
	}

	public override void OnHurt(Entity offender)
	{
		SFX.Instance.PlayFX("attack_flyingSkull", myTransform.position);

		if(sight.target == null)
		{
			sight.target = offender;
			_AIState = EnemyState.Chase;
		}

		if(firing) 
		{
			Destroy(currentFireball);
			StopCoroutine("FireballAttack");
		}

		if(charging)
		{
			charging = false;
			StopCoroutine("ChargeAttack");
		}

		SetTargetFacing();
		StartCoroutine("ChargeAttack");
	}

	public override IEnumerator Patrol()
	{
		speed = walkSpeed;
		//if the start location is left of me, face left
		SetFacing(startLocation.x < myTransform.position.x ? Facing.left : Facing.right);
		debugColor = Color.green;
		while(_AIState == EnemyState.Patrol)
		{
			if(Vector2.Distance(myTransform.position, startLocation) > 0.1f)
			{
				myTransform.position = Vector2.MoveTowards(myTransform.position, startLocation, speed * Time.fixedDeltaTime);
				yield return null;
			}
			else //we have arrived at start location. Dont move and face original facing.
			{
				SetFacing(startFacing);
			}
			yield return null;
		}
	}

	public override void FixedUpdate()
	{
		if(sight.target) 
		{
			Distance();
		}
		if(_health.Dead) magnitude = rbody.velocity.sqrMagnitude;
	}

	public override IEnumerator Chase()
	{
		speed = runSpeed;
		debugColor = Color.red;
		while(_AIState == EnemyState.Chase)
		{
			SetTargetFacing();

			//only update player position when chasing, not attacking
			if(!attacking)
			{
				//search if target is too far away
				if(sight.target == null)
				{
					_AIState = EnemyState.Search;
				}
				else
				{
					StartCoroutine("FireballAttack");
					yield return null;
				}
			}

			//if player is within attack range, run the attack method
			if (distance <= attackRange)
			{
				//only if caught in the fireball routine do we
				//stop the fireball routine and set attacking to false
				if(firing)
				{
					//wait until done firing, if firing...
					while(firing) yield return null;
					attacking = false;
					//stop the fireball shooting coroutine
					StopCoroutine("FireballAttack");
				}

				while(attacking) yield return null;
				StartCoroutine("ChargeAttack");
			}

			yield return null;
		}
	}

	public override void OnBecameInvisible()
	{
		if(!_health.Dead) return;
		Destroy(this.gameObject);
	}

	public override void OnDeath()
	{
		//stop coroutines
		StopCoroutine("AttackSequence");
		StopCoroutine("ChargeAttack");
		StopCoroutine("FireballAttack");
		StopCoroutine(_AIState.ToString()); //stop the current coroutine
		StopCoroutine("StateMachine"); //stop the coroutine state machine

		if(currentFireball) Destroy(currentFireball);

		rbody.isKinematic = false;
		rbody.bodyType = RigidbodyType2D.Dynamic;
		rbody.gravityScale = 1;
		rbody.velocity = Vector2.zero;
		rbody.mass = 100;

		//jaw
		jaw.transform.SetParent(null); //detach jaw
		jaw.bodyType = RigidbodyType2D.Dynamic;
		jaw.gravityScale = 1;
		jaw.mass = 100;

		//turn off chattering teeth asrc
		chatteringTeeth.Stop();

		//turn off particles
		fire.Stop();
		smoke.Stop();

		cam.UnRegisterMe(myTransform);
		myTrigger.enabled = false;
		myCollider.enabled = true;

		//change layers
		gameObject.layer = LayerMask.NameToLayer("TransparentFX");
		jaw.gameObject.layer = LayerMask.NameToLayer("TransparentFX");
		SFX.Instance.PlayFX("skeleton_death", transform.position);

		_anim.SetInteger("AnimState", 3);
		base.OnDeath();
		this.enabled = false;
	}

	public override void SetFacing(Facing face)
	{
		base.SetFacing(face);
	}

	public override IEnumerator Search()
	{
		float t = searchTime;
		debugColor = Color.yellow;
		while(_AIState == EnemyState.Search)
		{
			//wait x seconds before giving up and returning to start location
			while(t > 0 && !sight.CanPlayerBeSeen())
			{
				myTransform.position = Vector2.MoveTowards(myTransform.position,
						 targetPosition + new Vector3(0, riseHeight, 0), speed * Time.fixedDeltaTime);
						yield return null;
				//find target while waiting
				t -= Time.deltaTime;
				yield return null;
			}

			_AIState = EnemyState.Patrol;
			yield return null;
		}
	}

	void SetTargetFacing()
	{
		SetFacing(targetPosition.x < myTransform.position.x ? Facing.left : Facing.right);
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		// when the enemy dies then hits the ground, detach the jaw and drop stuff?
		if(!_health.Dead) return;
		//play bounce sound if magnitude of vertical impact exceeds threshold
		if(Mathf.Abs(magnitude) > bounceTolerance * rbody.mass)
		{
			SFX.Instance.PlayFX("hurt_skeleton", myTransform.position);
		}
		if(exploded) return;
		smoke.Emit(5);
		exploded = true;
		explode.OnExplode();
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if(col.CompareTag("Player"))
		{
			if(charging)
			{
				col.GetComponent<Health>().TakeDamage(this, 1);
			}
		}
	}

	IEnumerator FireballAttack()
	{
		attacking = true;

		int i = Random.Range(1, fireBalls);
		while(i > 0)
		{
			firing = true;

			//get target position once the moment before firing
			targetPosition = sight.target.Center;
			SetTargetFacing();

			currentFireball = Instantiate(fireballPrefab, fireballSpawnLocation.position, Quaternion.identity);
			Fireball myFireball = currentFireball.GetComponent<Fireball>();
			myFireball.myEntity = this;//set source of projectile to this

			//ignore collision between enemy and projectile
			Collider2D fireballCollider = currentFireball.GetComponent<Collider2D>();
			Physics2D.IgnoreCollision(fireballCollider, myCollider);
			Physics2D.IgnoreCollision(fireballCollider, jaw.GetComponent<Collider2D>());

			Rigidbody2D fireRbody = currentFireball.GetComponent<Rigidbody2D>();
			fireRbody.isKinematic = true;

			//charge up a fireball
			float t = fireballChargeTime;
			while(t > 0 && currentFireball)
			{
				float size = Mathf.Lerp(1, 0, t);
				currentFireball.transform.localScale = new Vector3(size, size, 1);
				t -= Time.deltaTime;
				yield return null;
			}

			//get target position once the moment before firing
			targetPosition = sight.target.Center;

			i--;
			_anim.SetTrigger("SpitFireball");
			fireRbody.isKinematic = false;
			SFX.Instance.PlayFX("fireball", fireballSpawnLocation.position);
			myFireball.StartFlight((targetPosition - fireballSpawnLocation.position).normalized);

			firing = false;
			yield return new WaitForSeconds(perFireballDelay);
		}

		yield return new WaitForSeconds (_health.HealthRatio < healthRatioLimit ? attackDelay/subRatioMultiplier : attackDelay);
		_anim.SetInteger("AnimState", 0); //windup then transition to attack

		//can now start new attack
		attacking = false;
	}

	IEnumerator ChargeAttack()
	{
		print("charge attack");
		attacking = true;

		SFX.Instance.PlayFX("windup_flyingSkull", myTransform.position);
		_anim.SetInteger("AnimState", 1); //windup then transition to attack

		//get target position once the moment before firing
		targetPosition = sight.target.Center;

		yield return new WaitForSeconds (0.5f); //delay before attacking

		speed = _health.HealthRatio < healthRatioLimit ? attackSpeed * subRatioMultiplier : attackSpeed;
		SFX.Instance.PlayFX("attack_flyingSkull", myTransform.position);

		//charge attack
		float t = chargeAttackDuration;

		charging = true;
		while(t > 0 && charging)
		{
			t-= Time.deltaTime;
			myTransform.position = Vector2.MoveTowards(myTransform.position, targetPosition, speed * Time.fixedDeltaTime);
			yield return null;
		}
		charging = false;

		_anim.SetInteger("AnimState", 0); //idle transition from windup
		//rise up to a height above the player
		float targetYposition = targetPosition.y + riseHeight;
		while(myTransform.position.y < targetYposition)
		{
			myTransform.position = Vector2.MoveTowards(myTransform.position, 
			new Vector3(myTransform.position.x, targetYposition, myTransform.position.z),
			speed * Time.fixedDeltaTime);
			yield return null;
		}

		yield return new WaitForSeconds (_health.HealthRatio < healthRatioLimit ? attackDelay/subRatioMultiplier : attackDelay);
		_anim.SetInteger("AnimState", 0); //windup then transition to attack

		//can now start new attack
		attacking = false;
	}

	//stop, telegraph attack, attack, then return to follow sequence
	//once within range, repeat
	IEnumerator Attack()
	{
		//dont move
		speed = 0;
		//the player is not targeted by any other enemies
		if(CameraController.Instance.m_Targets.Count < 3 && CameraController.Instance.m_Targets.Contains(this.transform))
		{
			if(_health.HealthRatio >= 0.5f && Random.value < fireballAttackChance)
			{
				StartCoroutine("FireballAttack");
			}
			else
			{
				StartCoroutine("ChargeAttack");
			}
		}
		//the player is already targeted by 1 or more enemies
		else
		{
			StartCoroutine("FireballAttack");
		}
		yield return null;
	}
}
