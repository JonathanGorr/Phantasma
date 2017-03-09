using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))] //used to destroy on invisible
public class EnemyAI : Enemy
{
	[Header("Enemy AI")]
	[HideInInspector] public StaggerStateBehaviour staggerState;

	[Header("Combat Maneuvers")]
	public bool canBackstep = false;
	public bool canBlock = false;
	public float backstepChance = .25f;
	public float blockChance = .25f;

	public float minYDistance = 3; //the minimum vertical distance the target must be to chase

	//add targets to this list that are, for example, beyond an impassable barrier
	List<Entity> ignoredTargets = new List<Entity>();

	public enum JumpConfidence { none, half, full, twice } //how high can a entity jump over an obstacle?

	[Header("Parms")]
	public JumpConfidence jumpConfidence = JumpConfidence.half;
	float[] jumps = new float[] { 0f, 0.5f, 1f, 2f }; //correlates to jump confidence
	
	public override void Awake()
	{
		base.Awake();

		startLocation = myTransform.position;
		startFacing = facing;
		StartCoroutine("StateMachine");
		cam = Camera.main.GetComponent<CameraController>();
	}

	public override void Subscribe()
	{
		base.Subscribe();
		if(_anim.GetBehaviour<StaggerStateBehaviour>())
		{
			staggerState = _anim.GetBehaviour<StaggerStateBehaviour>();
			staggerState.onEnter += OnStaggerEnter;
			staggerState.onExit += OnStaggerExit;
		}
	}

	public override void UnSubscribe()
	{
		base.UnSubscribe();
		if(target)
		{
			if(canBackstep) target.attackState.onEnter -= BackStep;
			if(canBlock) target.attackState.onEnter -= Block;
			target = null;
		}
		if(staggerState)
		{
			staggerState.onEnter -= OnStaggerEnter;
			staggerState.onExit -= OnStaggerExit;
		}
	}

	public virtual void OnStaggerEnter()
	{
		canMove = false;
		canFace = false;
	}
	public virtual void OnStaggerExit()
	{
		canMove = true;
		canFace = true;
	}

	//attack animation started
	public override void OnAttackEnter()
	{
		canMove = false;
		canFace = false;
		base.OnAttackEnter();
	}
	//attack animation ended
	public override void OnAttackExit()
	{
		canMove = true;
		canFace = true;
		StartCoroutine(Ready(attackDelay));
		base.OnAttackExit();
	}

	//used for horizontal obstacle detection
	public override void OnControllerColliderHorizontal (RaycastHit2D hit)
	{
		base.OnControllerColliderHorizontal(hit);

		if(target)
		{
			if(hit.collider.GetComponent<Destructable>())
			{
				Hack();
			}
			else
			{
				if(jumpConfidence != JumpConfidence.none)
				{
					if(_controller.isGrounded) //jump if grounded
					{
						//get my height and obstacle's height
						//decide whether to jump or repath based on how tall the obstacle is
						float size = hit.collider.bounds.size.y;
						float mySize = myTrigger.bounds.size.y;

						float heightDifference = hit.collider.transform.position.y - myTrigger.transform.position.y;
						print(heightDifference);

						float percent = size / mySize;

						print("My Size : " + mySize + ", " + "Obstacle Size : " + size + " = %" + Mathf.Round(percent*100) + " of my Size.");
						//if obstacle's height is 2x less than my height( i can jump my height), jump
						if(percent < jumps[(int)jumpConfidence])
						{
							Jump();
						}
						else
						{
							IgnoreTarget();
						}
					}
				}
				//has no jump confidence, give up on searching for target
				else
				{
					IgnoreTarget();
				}
			}
		}
	}

	public override void Force(Vector2 parm)
	{
		base.Force(parm);
	}

	public override Entity CircleCast()
	{
		Collider2D col = Physics2D.OverlapCircle(transform.position, sightRadius, layers);
		if(col != null) 
		{
			if(ignoredTargets.Contains(col.GetComponent<Entity>())) return null;
			return col.GetComponent<Entity>();
		}
		else return null;
	}

	void IgnoreTarget()
	{
		if(!ignoredTargets.Contains(target)) ignoredTargets.Add(target);

		//unsubscribe to the target's animations
		if(canBackstep) target.attackState.onEnter -= BackStep;
		if(canBlock) target.attackState.onEnter -= Block;

		target = null;
		if(_AIState == EnemyState.Chase) _AIState = EnemyState.Search;
	}

	public override void Jump()
	{
		if(!canMove) return;
		base.Jump();
	}

	public override void SetFacing(Facing face)
	{
		if(!canFace) return;
		base.SetFacing(face);
	}

	bool IsTargetLeft() //is the target left or right of me?
	{
		//player is on the right side
		if(!target) return false;
		return target.myTransform.position.x < myTransform.position.x;
	}

	//used to destroy obstacles
	void Hack()
	{
		if(!ready) return;
		if(attackState.inState) return;

		_anim.SetFloat("Combo", 2);
		_anim.SetTrigger("Attack");
	}

	public override void Attack()
	{
		if(!ready) return;
		if(attackState.inState) return;

		_anim.SetFloat("Combo", (int) Random.Range(0, 3));
		_anim.SetTrigger("Attack");

		base.Attack();
	}

	public virtual IEnumerator StateMachine()
	{
		while(true) 
		{
			//don't update if invisible
			if(!rend.isVisible) yield return null;
			yield return StartCoroutine(_AIState.ToString());
		}
	}

	public virtual IEnumerator Patrol()
	{
		speed = walkSpeed;
		ignoredTargets.Clear();
		SetFacing(myTransform.position.x < startLocation.x ? Facing.left : Facing.right);
		debugColor = Color.green;
		while(_AIState == EnemyState.Patrol)
		{
			//find target
			target = CircleCast();
			if(target) { _AIState = EnemyState.Chase; }

			//go home if not there already
			if(Mathf.Abs(myTransform.position.x - startLocation.x) > .1f) //only check x distance
			{
				SetFacing(_velocity.x < 0 ? Facing.left : Facing.right);
				normalizedHorizontalSpeed = myTransform.position.x > startLocation.x ? -1 : 1;
			}
			else //we have arrived at start location. Dont move and face original facing.
			{
				SetFacing(startFacing);
				normalizedHorizontalSpeed = 0;
			}

			yield return null;
		}
	}

	public virtual IEnumerator Chase()
	{
		//listen to the target's animations
		if(canBackstep) target.attackState.onEnter += BackStep;
		if(canBlock) target.attackState.onEnter += Block;

		cam.RegisterMe(myTransform);
		speed = runSpeed;
		debugColor = Color.red;
		while(_AIState == EnemyState.Chase)
		{
			if(target == null) 
			{
			 	_AIState = EnemyState.Search;
			 	yield break;
			}
			else //we have target
			{
				//too far to chase
				if(distance > chaseRange)
				{
					//listen to the target's animations
					if(canBackstep) target.attackState.onEnter -= BackStep;
					if(canBlock) target.attackState.onEnter -= Block;
					_AIState = EnemyState.Search;
					yield break;
				}
				else //we are in range
				{
					//if the target's y distance does not exceed value
					if(Mathf.Abs(target.transform.position.y - myTransform.position.y) <= minYDistance)
					{
						if(distance > minDistance)
						{
							if(canFace) SetFacing(IsTargetLeft() ? Facing.left : Facing.right);
						}
						//far enough to chase
						if(distance > attackRange)
						{
							if(canMove) normalizedHorizontalSpeed = IsTargetLeft() ? -1 : 1; //chase
						}
						//close enough to attack
						else if (distance <= attackRange)
						{
							normalizedHorizontalSpeed = 0; //stop and attack
							Attack();
						}
					}
					else
					{
						normalizedHorizontalSpeed = 0; //stop and attack
						//_AIState = EnemyState.Search;
					}
				}
			}
			yield return null;
		}
	}

	public virtual IEnumerator Search()
	{
		cam.UnRegisterMe(myTransform);
		ignoredTargets.Clear();
		float t = searchTime;
		debugColor = Color.yellow;
		while(_AIState == EnemyState.Search)
		{
			//wait x seconds before giving up and returning to start location
			while(t > 0 && target == null)
			{
				//find target while waiting
				target = CircleCast();
				t -= Time.deltaTime;
				yield return null;
			}

			_AIState = EnemyState.Patrol;
			yield return null;
		}
	}

	public override void Block()
	{
		base.Block();
	}

	public override void BackStep()
	{
		//if(attackState.inState) return; //don't backstep if already attacking
		if(Random.value > backstepChance) return;
		if(target) SetFacing(IsTargetLeft() ? Facing.left : Facing.right);
		base.BackStep();
	}

	public override void OnDeath()
	{
		cam.UnRegisterMe(myTransform);
		myTrigger.enabled = false;
		myCollider.enabled = false;
		ignoredTargets.Clear();
		StopCoroutine(_AIState.ToString()); //stop the current coroutine
		StopCoroutine("StateMachine"); //stop the coroutine state machine
		gameObject.layer = LayerMask.NameToLayer("TransparentFX");
		base.OnDeath();
		this.enabled = false;
	}

	public override void OnBecameInvisible()
	{
		if(_health.Dead)
		{
			Destroy(this.gameObject);
		}
	}

	public override void OnHurt(Entity offender)
	{
		if(target == null)
		{
			target = offender;
			_AIState = EnemyState.Chase;
		}
		normalizedHorizontalSpeed = 0;
		base.OnHurt(offender);
	}

	public virtual void Move()
	{
		if(_controller.collisionState.left || _controller.collisionState.right || !canMove)
		{
			normalizedHorizontalSpeed = 0;
		}

		//set speed for transitioning movement
		_anim.SetFloat ("Speed", Mathf.Abs(_velocity.x));
		_velocity.x = Mathf.Lerp( _velocity.x, normalizedHorizontalSpeed * speed, Time.deltaTime * damping );
		_velocity.y += gravity * Time.deltaTime; //apply gravity before moving
		_controller.move(_velocity * Time.deltaTime);
	}
	
	public override void FixedUpdate()
	{
		if(!rend.isVisible) return;
		if(target) Distance();
		base.FixedUpdate();
		Move();
	}
}