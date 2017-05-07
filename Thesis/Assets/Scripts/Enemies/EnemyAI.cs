using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))] //used to destroy on invisible
public class EnemyAI : Enemy
{
	[Header("Enemy AI")]
	[HideInInspector] public StaggerStateBehaviour staggerState;
	[HideInInspector] public SpecialAttackStateBehaviour specialState;

	[Header("Combat Maneuvers")]
	public bool canBackstep = false;
	public bool canBlock = false;
	public float backstepChance = .25f;
	public float blockChance = .25f;
	public float strongAttackChance = .25f;

	public float t = 0; //how long until the enemy enters patrol mode; returns home from searching

	public float minYDistance = 3; //the minimum vertical distance the target must be to chase

	//add targets to this list that are, for example, beyond an impassable barrier
	public enum JumpConfidence { none, half, full, twice } //how high can a entity jump over an obstacle?

	[Header("Parms")]
	public JumpConfidence jumpConfidence = JumpConfidence.half;
	float[] jumps = new float[] { 0f, 0.5f, 1f, 2f }; //correlates to jump confidence

	[Header("Special to Regular Attack Ratio")]
	public int regularToSpecialAttackLimit = 3; //how many regular attacks must  occur before another special attack can occur
	public int limit = 0;
	
	public override void Awake()
	{
		base.Awake();

		startLocation = myTransform.position;
		startFacing = facing;
		StartCoroutine("StateMachine");
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

		if(_anim.GetBehaviour<SpecialAttackStateBehaviour>())
		{
			specialState = _anim.GetBehaviour<SpecialAttackStateBehaviour>();
			specialState.onEnter += OnSpecialAttackEnter;
			specialState.onExit += OnSpecialAttackExit;
		}
	}
	public override void UnSubscribe()
	{
		base.UnSubscribe();

		if(staggerState)
		{
			staggerState.onEnter -= OnStaggerEnter;
			staggerState.onExit -= OnStaggerExit;
		}
		if(specialState)
		{
			specialState.onEnter -= OnSpecialAttackEnter;
			specialState.onExit -= OnSpecialAttackExit;
		}
	}

	//called by the lineofSight script when the player enters the collider and is visible to the enemy
	public override void FoundTarget(Entity e)
	{
		base.FoundTarget(e);

		SFX.Instance.PlayFX("alert_skeleton", myTransform.position);
		if(canBackstep)
		{
			e.attackState.onEnter += BackStep;
			e.specialAttackState.onEnter += BackStep;
		}
		if(canBlock)
		{
			e.attackState.onEnter += Block;
			e.specialAttackState.onEnter += Block;
		}
	}

	//called by the lineofSight script when the player has left the collider
	public override void LostTarget(Entity e)
	{
		if(canBackstep)
		{
			e.attackState.onEnter -= BackStep;
			e.specialAttackState.onEnter -= BackStep;
		}
		if(canBlock) 
		{
			e.attackState.onEnter -= Block;
			e.specialAttackState.onEnter -= Block;
		}
		base.LostTarget(e);
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

	public override void OnAttackEnter()
	{
		canMove = false;
		canFace = false;
		base.OnAttackEnter();
	}
	public override void OnAttackExit()
	{
		canMove = true;
		canFace = true;
		StartCoroutine(Ready(attackDelay));
		base.OnAttackExit();
	}

	public virtual void SpecialAttackEnter()
	{
		canMove = false;
		canFace = false;
	}
	public virtual void SpecialAttackExit()
	{
		canMove = true;
		canFace = true;
		StartCoroutine(Ready(attackDelay));
	}

	//used for horizontal obstacle detection
	public override void OnControllerColliderHorizontal (RaycastHit2D hit)
	{
		base.OnControllerColliderHorizontal(hit);
		//don't try and jump over entities
		if(hit.collider.gameObject.layer != LayerMask.NameToLayer(_controller.entitiesMask))
		{
			if(!_controller.collisionState.movingDownSlope)
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
							Debug.DrawLine(myTransform.position, new Vector3(hit.collider.bounds.center.x, hit.collider.bounds.max.y, 0), Color.blue);

							//get the jump height
							float obstacleHeight = hit.collider.bounds.max.y - myTransform.position.y;
							float myHeight = myTrigger.bounds.max.y - myTrigger.bounds.min.y;
							float percent = obstacleHeight / myHeight;

							//only jump when the actual obstacle is not walk-overable
							if(percent > .1f)
							{
								#if UNITY_EDITOR
								if(rend.isVisible) print("%" + Mathf.Round(percent * 100));
								#endif

								if(percent < jumps[(int)jumpConfidence])
								{
									Jump();
								}
								else
								{
									//IgnoreTarget();
								}
							}
						}
					}
					//has no jump confidence, give up on searching for target
					else
					{
						//IgnoreTarget();
					}
				}
			}
		}
	}

	public override void Jump()
	{
		if(!_controller.isGrounded) return;
		if(!canMove) return;
		base.Jump();
	}

	public bool IsTargetLeft() //is the target left or right of me?
	{
		//player is on the right side
		if(!sight.target) return false;
		return sight.target.myTransform.position.x < myTransform.position.x;
	}

	//used to destroy obstacles
	void Hack()
	{
		if(!ready) return;
		if(attackState.inState) return;

		_anim.SetFloat("Combo", 2);
		_anim.SetTrigger("Attack");
	}

	//am i facing the target?
	bool FacingTarget()
	{
		if(facing == Facing.left && sight.target.myTransform.position.x < myTransform.position.x)
			return true;
		else if(facing == Facing.right && sight.target.myTransform.position.x > myTransform.position.x)
			return true;
		
		return false;
	}

	public override void Attack()
	{
		if(!ready) return;
		if(!FacingTarget()) return;

		if(limit < regularToSpecialAttackLimit || Random.value > strongAttackChance || !canSpecialAttack)
		{
			_anim.SetFloat("Combo", (int) Random.Range(0, 3));
			_anim.SetTrigger("Attack");
			limit ++;
			return;
		}
		else
		{
			limit = 0;
			_anim.SetTrigger("StrongAttack");
			return;
		}
		base.Attack();
	}

	public override IEnumerator StateMachine()
	{
		while(true)
		{
			yield return StartCoroutine(_AIState.ToString());
		}
	}

	public override IEnumerator Patrol()
	{
		speed = walkSpeed;
		Facing = myTransform.position.x < startLocation.x ? Facing.left : Facing.right;

		while(_AIState == EnemyState.Patrol)
		{
			//if running back to origin and the player comes behind me too close, notice
			if(sight.target && sight.Distance <= attackRange)
			{
				_AIState = EnemyState.Chase;
			}
			//go home if not there already
			if(Mathf.Abs(myTransform.position.x - startLocation.x) > .1f) //only check x distance
			{
				Facing = _velocity.x < 0 ? Facing.left : Facing.right;
				normalizedHorizontalSpeed = myTransform.position.x > startLocation.x ? -1 : 1;
			}
			else //we have arrived at start location. Dont move and face original facing.
			{
				Facing =(startFacing);
				normalizedHorizontalSpeed = 0;
			}

			yield return null;
		}
	}

	public override IEnumerator Chase()
	{
		Facing = IsTargetLeft() ? Facing.left : Facing.right;
		speed = runSpeed;

		while(_AIState == EnemyState.Chase && sight.CanPlayerBeSeen())
		{
			//drop attack if falling from an adequate height
			if(_anim.GetBool("Falling") && height >= dropAttackHeight)
			{
				_anim.SetTrigger("Attack");
				//TODO: wait until grounded again( finished falling to ground ) to wait ~2 seconds, then return to normal behavior
				yield return null;
			}

			//if the sight.target's y distance does not exceed value
			if(Mathf.Abs(sight.target.transform.position.y - myTransform.position.y) <= minYDistance)
			{
				if(canFace) Facing = IsTargetLeft() ? Facing.left : Facing.right;

				//far enough to chase
				if(sight.Distance > attackRange)
				{
					if(canMove && !attackState.inState && !staggerState.inState && !backstepState.inState) normalizedHorizontalSpeed = IsTargetLeft() ? -1 : 1; //chase
					else normalizedHorizontalSpeed = 0;
				}
				//close enough to attack
				else if (sight.Distance <= attackRange)
				{
					//attack if not blocking, attacking or special attacking
					if(	!attackState.inState
					&&	!specialAttackState.inState
					&& 	!blockState.inState
					&& !backstepState.inState)
					{
						print("attack");
						Attack();
					}
				}
			}
			else
			{
				normalizedHorizontalSpeed = 0; //stop
			}

			yield return null;
		}
	}

	public override IEnumerator Search()
	{
		print("search");
		t = searchTime;

		//wait x seconds before giving up and returning to start location
		while(t > 0 && !sight.CanPlayerBeSeen())
		{
			//find target while waiting
			t -= Time.deltaTime;
			yield return null;
		}

		_AIState = EnemyState.Patrol;
		yield return null;
	}

	public override void Block()
	{
	}

	public override void BackStep()
	{
		if(Random.value > backstepChance) return;
		if(sight.Distance > attackRange) return;
		Force(backstep);
		if(sight.target) Facing = IsTargetLeft() ? Facing.left : Facing.right;
		base.BackStep();
	}

	public override void OnDeath()
	{
		StopCoroutine(_AIState.ToString()); //stop the current coroutine
		StopCoroutine("StateMachine"); //stop the coroutine state machine

		LostTarget(sight.target);
		UnSubscribe();

		CameraController.Instance.UnRegisterMe(myTransform);
		sight.enabled = false;
		myTrigger.enabled = false;
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
		//if hit unaware, stagger then aggro
		if(_AIState != EnemyState.Chase)
		{
			sight.target = offender;
			FoundTarget(offender);

			_anim.SetTrigger("Stagger");
			_anim.SetBool("Blocking", false);
		}

		//reset search time each time hit
		t = searchTime;
		//face the attacker on each hit
		Facing = IsTargetLeft() ? Facing.left : Facing.right;
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
		base.FixedUpdate();
		Move();
	}
}