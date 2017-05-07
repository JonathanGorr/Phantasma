using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using CharacterController;

/// <summary>
/// Is this entity facing left or right?
/// </summary>
public enum Facing { left, right }

/// <summary>
/// What combat state is this entity currently in: attacking, idle, jumping etc?
/// </summary>
public enum CombatState { Idle, Blocking, Aiming, Attacking, Rolling, BackStepping, Jumping }

/// <summary>
///	Root class for all variables shared by humans, npcs, and enemies
/// </summary>
public class Entity : MonoBehaviour, IEntity {

	public Facing facing; //the facing of this entity
	public virtual Facing Facing
	{
		get { return facing; }
		set 
		{
			//don't bother if already facing this direction
			if(facing == value) return;
			//don't face if not allowed
			if(!canFace) return;
			//set facing
			facing = value;
			//facing is determined by localscale.x
			transform.localScale = new Vector2(facing == Facing.left ? -Mathf.Abs(startScale.x) : Mathf.Abs(startScale.x), startScale.y);
		}
	}


	[Header("Entity")]
	public string title;
	public string Title
	{
		get { return title; }
		set { title = value; }
	}
	public string description;
	public string Description
	{
		get { return description; }
		set { description = value; }
	}

	public Transform body;
	public Transform head;

	[Header("Rotation")]
	public float angleOffset = 0f;
	public Vector2 rotOffset;
	public float rotLerpSpeed = 2;

	Vector2 lookDirection = Vector2.right;
	public Vector2 LookDirection
	{
		get { return lookDirection; }
		set { lookDirection = value; }
	}

	//backstepping
	[Header("Forces")]
	public Vector2 jump = new Vector2(0.0f, 2.0f);
	public Vector2 backstep = new Vector2(400.0f, 0.8f);
	public Vector2 attack = new Vector2(45.0f, 0.0f);
	public Vector2 slideBlocking = new Vector2(-45.0f, 0.0f);
	public Vector2 slidingAttack = new Vector2(-200.0f, 0.0f);
	public Vector2 roll = new Vector2(-400.0f, 0.8f);
	public Vector2 leapAttack = new Vector2(-100.0f, 1.5f);

	[Header("Delays")]
	public float attackDelay = .3f;
	public float rollDelay = 0.65f;
	public float backStepDelay = 0.65f;
	public float jumpDelay = 0.3f;
	public float blockAttackDelay = 0.5f;

	[Header("Refs")]
	public Health _health;
	public Transform myTransform;
	public Animator _anim;
	public AnimationMethods animMethods;
	public ParticleSystem _footDust;
	public CharacterController2D _controller;
	public Stamina _stamina;
	public CombatState combatState;
	public Collider2D myTrigger;

	//anim state behaviours
	[HideInInspector] public AttackStateBehaviour attackState;
	[HideInInspector] public SpecialAttackStateBehaviour specialAttackState;
	[HideInInspector] public BlockStateBehaviour blockState;
	[HideInInspector] public BackstepStateBehaviour backstepState;
	[HideInInspector] public JumpStateBehaviour jumpState;
	[HideInInspector] public RollStateBehaviour rollState;

	[Header("Movement Parms")]
	[HideInInspector] public bool moving = false;
	public bool canMove = true;
	public bool canFace = true;
	public bool canJump = true;
	public bool canDropAttack = false;
	public bool canSpecialAttack = false;
	[HideInInspector] public float damping; // how fast do we change direction? higher means faster
	public float actionDamping = 3f;
	public float inAirDamping = 20f;
	public float groundDamping = 8f;
	[HideInInspector] public Vector3 _velocity;
	[HideInInspector] public bool ready = true;

	[Header("Height Detection")]
	public float killHeight = 10;
	public float dropAttackHeight = 3;
	public float height;
	public RaycastHit2D heightHit;
	public LayerMask groundLayers;

	[Header("Speeds")]
	public float walkSpeed = 3f;
	public float runSpeed = 6f;
	public float runSpeedHeavy = 4f;
	public float blockSpeed = 0f;
	[HideInInspector] public float speed;
	public float gravity = -25;

	[HideInInspector] public Vector2 startScale;

	//listen to entity death
	public delegate void OnDied();
	public event OnDied died;
	//Must manually set instance if called in derived classes
	protected virtual void HasDied()
	{
		OnDied hasDied = died;
		if(hasDied != null) hasDied();
	}

	public Vector3 Center
	{
		get { return myTrigger.bounds.center; }
	}

	public float normalizedHorizontalSpeed = 0;

	public bool Moving
	{
		get { return !(normalizedHorizontalSpeed == 0); }
	}

	public virtual void Awake()
	{
		SetDamping(groundDamping);

		if(!myTransform) myTransform = transform;
		startScale = myTransform.localScale;
		//facing = myTransform.localScale.x < 0 ? Facing.left : Facing.right;

		Subscribe();
	}

	public virtual void Start() { }
	public virtual void OnDisable() { }

	//used for horizontal obstacle detection
	public virtual void OnControllerColliderHorizontal (RaycastHit2D hit) { }

	public virtual void OnAttackEnter()  
	{
		SetDamping(actionDamping);
		canMove = false;
		canFace = false;
		StartCoroutine(Ready(attackDelay)); 
	}
	public virtual void OnAttackExit()  
	{
		SetDamping(groundDamping);
		canMove = true;
		canFace = true;
	}

	public virtual void OnSpecialAttackEnter()  
	{
		SetDamping(actionDamping);
		canMove = false;
		canFace = false;
		StartCoroutine(Ready(attackDelay)); 
	}
	public virtual void OnSpecialAttackExit()  
	{
		SetDamping(groundDamping);
		canMove = true;
		canFace = true;
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
		StartCoroutine(Ready(backStepDelay));
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
		SetDamping(groundDamping);
	}

	/// <summary>
	/// Subscribes to all animation behaviours and component events.
	/// </summary>
	public virtual void Subscribe()
	{
		_health.onHurt += OnHurt;
		_health.onDeath += OnDeath;

		//subscribe to behaviours relevant to this entity's combat
		if(_anim.GetBehaviour<AttackStateBehaviour>())
		{
			attackState = _anim.GetBehaviour<AttackStateBehaviour>();
			attackState.onEnter += OnAttackEnter;
			attackState.onExit += OnAttackExit;
		}
		if(_anim.GetBehaviour<SpecialAttackStateBehaviour>())
		{
			specialAttackState = _anim.GetBehaviour<SpecialAttackStateBehaviour>();
			specialAttackState.onEnter += OnSpecialAttackEnter;
			specialAttackState.onExit += OnSpecialAttackExit;
		}
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

		if(_controller) _controller.onControllerCollidedEventHorizontal += OnControllerColliderHorizontal;
	}

	public virtual void UnSubscribe()
	{
		_health.onHurt -= OnHurt;
		_health.onDeath -= OnDeath;

		animMethods.onLunge -= Lunge;
		animMethods.onLeap -= Leap;
		animMethods.onBackstep -= BackstepForce;

		if(backstepState)
		{
			backstepState.onEnter -= OnBackstepEnter;
			backstepState.onExit -= OnBackstepExit;
		}
		if(specialAttackState)
		{
			specialAttackState.onEnter -= OnSpecialAttackEnter;
			specialAttackState.onExit -= OnSpecialAttackExit;
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
		if(_controller) _controller.onControllerCollidedEventHorizontal -= OnControllerColliderHorizontal;
	}

	public void SetPosition(Vector2 pos)
	{
		myTransform.position = pos;
	}

	public virtual void OnHurt(Entity offender)
	{
		//if blocking, damage is half and you dont get knocked back
		if(blockState.inState)
		{
			SlideBlocking();
			return;
		}
		if(CanForce()) BackStep();
	}

	//slide back from getting hit by a weapon when shielded
	public virtual void SlideBlocking()
	{
		//if(_stamina) _stamina.UseStamina(blockedHitDrain);
		SFX.Instance.PlayFX("block", transform.position);
		if(CanForce()) Force(slideBlocking);
		SFX.Instance.PlayFX("slide_blocking", myTransform.position);
	}

	public virtual void Force(Vector2 parm)
	{
		_velocity.y = Mathf.Sqrt(Mathf.Abs(parm.y) * -gravity);
		_velocity.x = facing == Facing.left ? Mathf.Sqrt(Mathf.Abs(parm.x)) : -Mathf.Sqrt(Mathf.Abs(parm.x));
		if(parm.x < 0) _velocity.x *= -1;
		if(parm.y < 0) _velocity.y *= -1;
		_controller.move(_velocity * Time.deltaTime);
	}

	public virtual bool CanForce()
	{
		//if you're trying to roll right but you're blocked
		if(facing == Facing.right && _controller.collisionState.right) return false;
		//if you're trying to roll left and you're blocked
		if(facing == Facing.left && _controller.collisionState.left) return false;
		//not blocked or rolling in the opposite direction of a block...
		return true;
	}

	/// <summary>
	/// Perform a backstep.
	/// </summary>
	public virtual void BackstepForce()
	{
		if(CanForce()) Force(backstep);
	}

	/// <summary>
	/// Perform a jump.
	/// </summary>
	public virtual void Jump()
	{
		Force(jump);
	}

	/// <summary>
	/// Perform a lunge.
	/// </summary>
	public virtual void Lunge()
	{
		if(CanForce()) Force(attack);
	}

	/// <summary>
	/// Perform a leap.
	/// </summary>
	public virtual void Leap()
	{
		if(CanForce()) Force(leapAttack);
	}

	/// <summary>
	/// Rotates the body sprite.
	/// </summary>
	public virtual void RotateBody()
	{
		//transform the lookDirection v2 into an angle using Atan2
		float lookRotation = Mathf.Atan2(
		facing == Facing.left ? -lookDirection.y : lookDirection.y,
		facing == Facing.left ? -lookDirection.x : lookDirection.x) * Mathf.Rad2Deg;
		//normalized distance of lookdirection
		float normalizedDistance = lookDirection.sqrMagnitude;
		//if the lookDirection exceeds a minimum threshold...
		if(normalizedDistance > 0.05f)
		{
			//set facing to clamped direction depending on facing
			float angle = (facing == Facing.left ? (lookRotation + rotOffset.x) : (lookRotation + rotOffset.y));
			angle = facing == Facing.left ? Mathf.Clamp(angle, -80, 30) : Mathf.Clamp(angle, -30, 80);
			body.rotation = Quaternion.Euler(new Vector3(
			body.rotation.eulerAngles.x, 
			body.rotation.eulerAngles.y,
			angle));
		}
		//does not exceed threshold
		else
		{
			body.rotation = Quaternion.Euler(new Vector3(0,0, facing == Facing.left ? rotOffset.x : rotOffset.y ));
		}
	}

	/// <summary>
	/// Detects how high the entity currently is above the ground.
	/// Is used to detect whether an entity can perform a drop attack, as they can simply from a jump, currently.
	/// </summary>
	public virtual void HeightDetection()
	{
		heightHit = Physics2D.Raycast(myTransform.position, Vector2.down, Mathf.Infinity, groundLayers);
		if(heightHit.collider != null)
		{
			height = heightHit.distance;

			if(height >= dropAttackHeight)
				canDropAttack = true;
			else
				canDropAttack = false;

			if(height >= killHeight)
				Debug.DrawLine(myTransform.position, heightHit.point, Color.red);
			else
				Debug.DrawLine(myTransform.position, heightHit.point, Color.green);
		}
	}

	public virtual void Update()
	{
		HeightDetection();
		moving = Moving;
		//dont move down if grounded
		if(_controller) { if( _controller.isGrounded) { _velocity.y = 0; } }
	}

	public virtual void LateUpdate()
	{
	}

	public virtual void Idle()
	{
		combatState = CombatState.Idle;
	}

	public virtual void Attack()
	{
		if(combatState != CombatState.Blocking)
		{
			combatState = CombatState.Attacking;
		}
	}

	public virtual void Block()
	{
		combatState = CombatState.Blocking;
	}

	public virtual void Aim()
	{
		combatState = CombatState.Aiming;
	}

	public virtual void Roll()
	{
		combatState = CombatState.Rolling;
		if(CanForce()) Force(roll);
	}

	public virtual void BackStep()
	{
		_anim.SetTrigger("BackStep");
		combatState = CombatState.BackStepping;
	}

	public virtual void SlideAttack()
	{
		SFX.Instance.PlayFX("slide", myTransform.position);
	}

	public virtual void OnDeath()
	{
		HasDied();
		UnSubscribe();
		canMove = false;
		canFace = false;
		_velocity.x = 0;
		_anim.SetTrigger("Collapse");
	}

	public virtual void FixedUpdate()
	{
		_anim.SetBool("Falling", !_controller.isGrounded);
	}

	public virtual void OnBecameInvisible()
	{
	}

	public virtual void OnBecameVisible()
	{
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

	//Stamina
	public IEnumerator Ready(float delay)
	{
		if(delay == 0) yield break;
		ready = false;
		yield return new WaitForSeconds (delay);
		ready = true;
	}
}
