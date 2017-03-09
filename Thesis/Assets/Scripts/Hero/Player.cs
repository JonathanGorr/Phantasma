using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Player : Entity
{
	public static Player Instance = null;

	[Header("Local References")]
	public WeaponController _switcher;

	[HideInInspector] public UI _ui;
	[HideInInspector] public CameraController _cam;

	private SlideAttackStateBehaviour slideAttackState;
	private DropAttackStateBehaviour dropAttackState;
	private SpecialAttackStateBehaviour specialAttackState;
	private BlockAttackStateBehaviour blockAttackState;

	//flashing
	private Flash _flash;
	[Header("Flash")]
	public int flashes = 6;
	public float flashDuration = 0.05f;
	public bool disable;
	private SpriteRenderer[] _sprites;
	public float acceleration = 2; //how fast we get to top run speed from a standing speed
	public float deceleration = 4; //how fast we slow down on no input
	public float platformDropAnalogTolerance = 0.05f;

	public override void Awake()
	{
		if(Instance == null) Instance = this;
		base.Awake();

		//register player if not already
		CameraController.Instance.RegisterMe(myTransform);
	}

	public override void Start()
	{
		base.Start();

		//register methods to input events
		PlayerInput.onR1 += Attack;
		PlayerInput.onRightTrigger += StrongAttack;
		PlayerInput.onA += Jump;
		PlayerInput.onB += Roll;
		PlayerInput.onX += BackStep;
		PlayerInput.onY += _health.Heal;

		//delegate initialization
		SetSpeed(walkSpeed);
		SetDamping(groundDamping);
		
		//import components
		_flash = GetComponent<Flash>();
		_sprites = GetComponentsInChildren<SpriteRenderer>();
		_ui = LevelManager.Instance.transform.Find("UI").GetComponent<UI>();
		_cam = Camera.main.GetComponent<CameraController>();
		_cam.RegisterMe(myTransform);
	}

	public override void Subscribe()
	{
		ConversationManager.onConversationStarted += Stop;
		ConversationManager.onConversationEnded += Go;

		specialAttackState = _anim.GetBehaviour<SpecialAttackStateBehaviour>();
		specialAttackState.onEnter += SpecialAttackEnter;
		specialAttackState.onExit += SpecialAttackExit;

		slideAttackState = _anim.GetBehaviour<SlideAttackStateBehaviour>();
		slideAttackState.onEnter += SlideAttackEnter;
		slideAttackState.onExit += SlideAttackExit;
		animMethods.onSlide += SlideAttack;

		dropAttackState = _anim.GetBehaviour<DropAttackStateBehaviour>();
		dropAttackState.onEnter += DropAttackEnter;
		dropAttackState.onExit += DropAttackExit;

		blockAttackState = _anim.GetBehaviour<BlockAttackStateBehaviour>();
		blockAttackState.onEnter += BlockAttackEnter;
		blockAttackState.onExit += BlockAttackExit;

		base.Subscribe();
	}
	public override void UnSubscribe()
	{
		ConversationManager.onConversationStarted -= Stop;
		ConversationManager.onConversationEnded -= Go;
	
		slideAttackState.onEnter -= SlideAttackEnter;
		slideAttackState.onExit -= SlideAttackExit;
		animMethods.onSlide -= SlideAttack;

		dropAttackState.onEnter -= DropAttackEnter;
		dropAttackState.onExit -= DropAttackExit;

		specialAttackState.onEnter -= SpecialAttackEnter;
		specialAttackState.onExit -= SpecialAttackExit;

		blockAttackState.onEnter -= BlockAttackEnter;
		blockAttackState.onExit -= BlockAttackExit;

		base.UnSubscribe();
	}

	void Stop()
	{
		normalizedHorizontalSpeed = 0;
		_anim.SetFloat("Speed", 0);
		canMove = false;
		canFace = false;
		_velocity.x = 0;
	}
	void Go()
	{
		canMove = true;
		canFace = true;
	}

	void BlockEnter()
	{
		canMove = false;
		canFace = false;
	}
	void BlockExit()
	{
		SetDamping(groundDamping);
		canMove = true;
		canFace = true;
	}

	void BlockAttackEnter()
	{
		SetDamping(actionDamping);
		canMove = false;
		canFace = false;
		StartCoroutine(Ready(blockAttackDelay));
	}
	void BlockAttackExit()
	{
		canMove = true;
		canFace = true;
	}

	void SpecialAttackEnter()
	{
		SetDamping(actionDamping);
		canMove = false;
		canFace = false;
		StartCoroutine(Ready(attackDelay));
	}
	void SpecialAttackExit()
	{
		SetDamping(groundDamping);
		canMove = true;
		canFace = true;
	}

	void DropAttackEnter()
	{
		normalizedHorizontalSpeed = 0;
		canMove = false;
		canFace = false;
		//remove enemy collision while drop attacking
		_controller.platformMask ^= (1<<LayerMask.NameToLayer("Enemies"));
	}
	void DropAttackExit()
	{
		canMove = true;
		canFace = true;
		//restore enemy collision after drop attacking
		_controller.platformMask ^= (1<<LayerMask.NameToLayer("Enemies"));
	}

	public override void OnDisable()
	{
		base.OnDisable();
	}

	public void SlideAttackEnter()
	{
		SetDamping(actionDamping);
		canMove = false;
		canFace = false;
	}
	public void SlideAttackExit()
	{
		SetDamping(groundDamping);
		canMove = true;
		canFace = true;
	}

	//triggers when blocking and hit by an attack
	public override void SlideAttack()
	{
		Force(slidingAttack);
		base.SlideAttack();
	}

	public override void OnHurt(Entity offender)
	{
		base.OnHurt(offender);
		SFX.Instance.PlayFX("player_Hurt", myTransform.position);
		//start the flashing when hurt
		StartCoroutine(_flash.FlashSprites(_sprites, flashes, flashDuration, disable));
		if(combatState == CombatState.Blocking)
		{
			_stamina.Drain(_stamina.blockDrain);
		}
	}

	public override void OnDeath ()
	{
		CameraController.Instance.UnRegisterMe(transform);
		base.OnDeath ();
		normalizedHorizontalSpeed = 0;
	}

	void Movement()
	{
		_controller.ignoreOneWayPlatformsThisFrame = PlayerInput.Instance.LAnalog.y < -platformDropAnalogTolerance;

		//if pushing right on the joystick...
		if(PlayerInput.Instance.LAnalog.x > 0.1f || PlayerInput.Instance.LAnalog.x < -0.1f)
		{
			if(!ConversationManager.Instance.talking)
			{
				if(canMove && !slideAttackState.inState) 
				{
					normalizedHorizontalSpeed += (PlayerInput.Instance.LAnalog.x < 0 ? -Time.deltaTime : Time.deltaTime)
					 * acceleration * Mathf.Abs(PlayerInput.Instance.LAnalog.x);

					if(facing == Facing.left){ normalizedHorizontalSpeed = Mathf.Clamp(normalizedHorizontalSpeed, -1, 0); }
					else { normalizedHorizontalSpeed = Mathf.Clamp(normalizedHorizontalSpeed, 0, 1); }
				}
			}
			SetFacing(PlayerInput.Instance.LAnalog.x > 0 ? Facing.right : Facing.left);
		}
		//keyboard movement
		/*
		//else if pushing left on the keyboard
		else if(PlayerInput.LAnalog.x == -1) //if walking left
		{
			//animation speed keyboard
			_anim.SetFloat ("Speed", 1);
			normalizedHorizontalSpeed = -1;
			SetFacing(true);
			if( _controller.isGrounded)
				_anim.SetInteger("AnimState", 1);
		}

		//else if pushing right on the keyboard...
		else if(PlayerInput.LAnalog.x > 0) //if walking right
		{
			//animation speed keyboard
			_anim.SetFloat ("Speed", 1);
			normalizedHorizontalSpeed = 1;
			SetFacing(false);
			if( _controller.isGrounded) _anim.SetInteger("AnimState", 1);
		}
		*/
		//else if no input, stand still...
		else
		{
			if(Mathf.Abs(normalizedHorizontalSpeed) > 0.05f) 
				normalizedHorizontalSpeed += (normalizedHorizontalSpeed > 0 ? -Time.deltaTime : Time.deltaTime) * deceleration;
			else normalizedHorizontalSpeed = 0;

			if( _controller.isGrounded) _anim.SetInteger("AnimState", 0);
		}

		_anim.SetFloat("Speed", Mathf.Abs(normalizedHorizontalSpeed));
		//apply horizontal speed smoothing ------------------------------------------------------------------------------
		_velocity.x = Mathf.Lerp( _velocity.x, normalizedHorizontalSpeed * speed, Time.deltaTime * damping );
		//apply gravity before moving
		_velocity.y += gravity * Time.deltaTime;
		_controller.move( _velocity * Time.deltaTime);
	}

	public override void Update()
	{
		base.Update();
		if(combatState == CombatState.Blocking) SetFacing(PlayerInput.Instance.RAnalog.x < 0 ? Facing.left : Facing.right);
		// grab our current _velocity to use as a base for all calculations
		_velocity = _controller.velocity;
		//input actions
		Actions();
	}

	void Actions()
	{
		if(!CanAct()) return;

		if(PlayerInput.Instance.L1Down) { Block(); }
		else { Idle(); }
	}

	public override void Idle()
	{
		base.Idle();
		_health.blocking = false;
		SetSpeed (walkSpeed);
	}

	public bool CanAct()
	{
		if(_health.Dead) return false;
		if(!ready) return false;
		if(!_controller.isGrounded) return false;
		//if(ConversationManager.Instance.talking) return false;
		if(PauseMenu.paused) return false;
		return true;
	}

	public override void Block()
	{
		if(_stamina.CurrentStamina == 0) return;
		//base.Block();
		combatState = CombatState.Blocking;
		_health.blocking = true;
		LookDirection = PlayerInput.Instance.RAnalog;
		SetSpeed (blockSpeed);
	}

	public override void Jump()
	{
		if(ConversationManager.Instance.talking) return;
		if(SceneManager.GetActiveScene().name == "Menu") return;
		if(!CanAct()) return;

		SFX.Instance.PlayFX("jump", myTransform.position);
		base.Jump();
	}

	public override void Roll()
	{
		if(!CanAct()) return;
		if(!_stamina.Ready) return;
		if(_switcher.IsWeapon(Weapons.Spear)) return;
		if(combatState == CombatState.Blocking) return;

		base.Roll();

		_anim.SetTrigger("Roll");
		SFX.Instance.PlayFX("jump", myTransform.position);
	}

	public override void Attack()
	{
		//if(ConversationManager.Instance.talking) return;
		if(PauseMenu.paused) return;
		if(!_stamina.Ready) return;
		//can't attack without a weapon
		if(_switcher.IsWeapon(Weapons.Empty)) return;
		if(_switcher.IsWeapon(Weapons.Bow)) return;

		if(!ready) return;

		//falling attack
		if(_anim.GetBool("Falling"))// && !jumpState.inState)
		{
			
			_anim.SetTrigger("Attack");
			return;
		}

		if(!_controller.isGrounded) return;

		//slide attack
		if(Mathf.Abs(normalizedHorizontalSpeed) >= .95f //are we running at ~max speed?
		&& combatState != CombatState.Blocking) //are we not blocking?
		{
			_anim.SetTrigger("SlidingAttack");
			return;
		}

		base.Attack();

		if(combatState == CombatState.Blocking)
		{
			_anim.SetTrigger("BlockingAttack");
		}
		else
		{
			combatState = CombatState.Attacking;
			_anim.SetTrigger("Attack");
		}

		SetSpeed (blockSpeed);
	}

	void StrongAttack()
	{
		if(!_stamina.Ready) return;
		if(!CanAct()) return;

		//can't attack without a weapon
		if(_switcher.IsWeapon(Weapons.Empty)) return;
		if(Mathf.Abs(normalizedHorizontalSpeed) > .1f && combatState != CombatState.Blocking) return;

		SetSpeed (blockSpeed);
		_anim.SetTrigger("StrongAttack");
	}

	public override void BackStep()
	{
		if(!CanAct()) return;
		if(Mathf.Abs(normalizedHorizontalSpeed) > .1f) return;
		if(!_switcher.weapon.canBackStep) return;

		base.BackStep();
		_anim.SetTrigger("BackStep");
		SFX.Instance.PlayFX("jump", myTransform.position);
	}

	//handle movement in fps time
	public override void FixedUpdate()
	{
		base.FixedUpdate();
		Movement();
	}
}
