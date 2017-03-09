using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterController;

/// <summary>
/// Humanoid:
/// Any entity that has 2 feet or otherwise uses the charactercontroller2d
// Includes: player, skeletons, boss etc
// does not include: flying enemies
/// </summary>

[RequireComponent(typeof (CharacterController2D))]
public class Humanoid : Entity {

/*

	[Header("Refs")]
	public ParticleSystem _footDust;
	public CharacterController2D _controller;

	public Transform head;

	[Header("Forces")]
	public Vector2 slideBlocking = new Vector2(45.0f, 0.0f);
	public Vector2 slidingAttack = new Vector2(-150.0f, 0.0f);
	public Vector2 roll = new Vector2(-250.0f, 0.8f);
	public Vector2 leapAttack = new Vector2(-170.0f, 1f);
	public Vector2 jump = new Vector2(0.0f, 3.0f);
	public Vector2 backstep = new Vector2(200.0f, 0.8f);

	[Header("Delays")]
	public float rollDelay = 0.65f;
	public float backStepDelay = 0.65f;
	public float jumpDelay = 0.3f;
	public float blockAttackDelay = 0.5f;

	public float actionDamping = 4f;
	public float inAirDamping = 20f;
	public float groundDamping = 8f;

	[Header("Speeds")]
	public float blockSpeed = 0f;
	public float gravity = -25;

	[HideInInspector] public float damping; // how fast do we change direction? higher means faster

	[HideInInspector] public BlockStateBehaviour blockState;
	[HideInInspector] public BackstepStateBehaviour backstepState;
	[HideInInspector] public JumpStateBehaviour jumpState;
	[HideInInspector] public RollStateBehaviour rollState;

	[HideInInspector] public Vector3 _velocity;

	public override void Awake()
	{
		base.Awake();

		SetDamping(groundDamping);
	}

	public override void Subscribe()
	{
		base.Subscribe();

		if(_anim.GetBehaviour<BlockStateBehaviour>())
		{
			blockState = _anim.GetBehaviour<BlockStateBehaviour>();
			blockState.onEnter += OnBlockEnter;
			blockState.onExit += OnBlockExit;
		}
		if(_anim.GetBehaviour<BackstepStateBehaviour>())
		{
			backstepState = _anim.GetBehaviour<BackstepStateBehaviour>();
			backstepState.onEnter += OnBackstepEnter;
			backstepState.onExit += OnBackstepExit;
		}
		if(_anim.GetBehaviour<JumpStateBehaviour>())
		{
			jumpState = _anim.GetBehaviour<JumpStateBehaviour>();
			jumpState.onEnter += OnJumpEnter;
			jumpState.onExit += OnJumpExit;
		}
		if(_anim.GetBehaviour<RollStateBehaviour>())
		{
			rollState = _anim.GetBehaviour<RollStateBehaviour>();
			rollState.onEnter += OnRollEnter;
			rollState.onExit += OnRollExit;
		}

		//force anims
		animMethods.onLunge += Lunge;
		animMethods.onLeap += Leap;
		animMethods.onBackstep += BackstepForce;

		_controller.onControllerCollidedEventHorizontal += OnControllerColliderHorizontal;
	}

	public override void UnSubscribe()
	{
		base.UnSubscribe();

		if(backstepState)
		{
			backstepState.onEnter -= OnBackstepEnter;
			backstepState.onExit -= OnBackstepExit;
		}
		if(blockState)
		{
			blockState.onEnter -= OnBlockEnter;
			blockState.onExit -= OnBlockExit;
		}
		if(attackState)
		{
			attackState.onEnter -= OnAttackEnter;
			attackState.onExit -= OnAttackExit;
		}
		if(jumpState)
		{
			jumpState.onEnter -= OnJumpEnter;
			jumpState.onExit -= OnJumpExit;
		}
		if(rollState)
		{
			rollState.onEnter -= OnRollEnter;
			rollState.onExit -= OnRollExit;
		}

		animMethods.onLunge -= Lunge;
		animMethods.onLeap -= Leap;
		animMethods.onBackstep -= BackstepForce;

		_controller.onControllerCollidedEventHorizontal -= OnControllerColliderHorizontal;
	}

	public override void Update()
	{
		base.Update();

		//dont move down if grounded
		if(_controller) { if( _controller.isGrounded) { _velocity.y = 0; } }
	}


	public override void OnDeath()
	{
		base.OnDeath();
		_velocity.x = 0;
		_anim.SetTrigger("Collapse");
	}

	public override void FixedUpdate()
	{
		base.FixedUpdate();
		_anim.SetBool("Falling", !_controller.isGrounded);
	}

	public virtual void Block()
	{
		combatState = CombatState.Blocking;
	}

	public virtual void Roll()
	{
		combatState = CombatState.Rolling;
		Force(roll);
	}

	public virtual void BackStep()
	{
		combatState = CombatState.BackStepping;
	}

	public virtual void SlideAttack()
	{
		SFX.Instance.PlayFX("slide", myTransform.position);
	}

	public virtual void BackstepForce()
	{
		Force(backstep);
	}
	public virtual void Jump()
	{
		Force(jump);
	}
	public virtual void Lunge()
	{
		Force(attack);
	}
	public virtual void Leap()
	{
		Force(leapAttack);
	}

	//takes a float speed num, changes speed based on input
	public virtual void SetDamping(float dmp)
	{
		damping = dmp;
	}

	//takes a float speed num, changes speed based on input
	public virtual void SetSpeed(float num)
	{
		speed = num;
	}

	public override void Force(Vector2 parm)
	{
		//don't move if colliding
		if(_controller.collisionState.left || _controller.collisionState.right) { return; }

		_velocity.y = Mathf.Sqrt(Mathf.Abs(parm.y) * -gravity);
		_velocity.x = facing == Facing.left ? Mathf.Sqrt(Mathf.Abs(parm.x)) : -Mathf.Sqrt(Mathf.Abs(parm.x));
		if(parm.x < 0) _velocity.x *= -1;
		if(parm.y < 0) _velocity.y *= -1;
		_controller.move(_velocity * Time.deltaTime);
	}

	public override void OnHurt(Entity offender)
	{
		//if blocking, damage is half and you dont get knocked back
		if(combatState == CombatState.Blocking)
		{
			SlideBlocking();
			return;
		}
		BackStep();
		base.OnHurt(offender);
	}

	//slide back from getting hit by a weapon when shielded
	public virtual void SlideBlocking()
	{
		//if(_stamina) _stamina.UseStamina(blockedHitDrain);
		SFX.Instance.PlayFX("block", transform.position);
		Force(slideBlocking);
		SFX.Instance.PlayFX("slide_blocking", myTransform.position);
	}

	public virtual void OnRollEnter()
	{
		SetDamping(actionDamping);
		canMove = false;
		canFace = false;
		StartCoroutine(Ready(rollDelay)); 
	}
	public virtual void OnRollExit()
	{
		SetDamping(groundDamping);
		canMove = true;
		canFace = true;
	}

	public virtual void OnBlockEnter()   
	{
		canMove = false;
	}
	public virtual void OnBlockExit()    
	{
		canMove = true;
	}

	public virtual void OnBackstepEnter()
	{
		SetDamping(actionDamping);
		canMove = false;
		canFace = false; 
		StartCoroutine(Ready(blockAttackDelay));
	}
	public virtual void OnBackstepExit() 
	{
		SetDamping(groundDamping);
		canMove = true;
		canFace = true;
	}
	public virtual void OnJumpEnter()
	{
		SetDamping(inAirDamping);
	}
	public virtual void OnJumpExit()
	{
	}
	*/
}
