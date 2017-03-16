using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Player : Entity
{
	public static Player Instance = null;

	public ParticleSystem sparks;

	[Header("Local References")]
	public WeaponController _switcher;

	private SlideAttackStateBehaviour slideAttackState;
	private DropAttackStateBehaviour dropAttackState;
	private BlockAttackStateBehaviour blockAttackState;

	//flashing
	[Header("Flash")]
	public int flashes = 6;
	private Flash _flash;
	public float flashDuration = 0.05f;
	public bool disable;
	private SpriteRenderer[] _sprites;

	[Header("Movement")]
	public float acceleration = 2; //how fast we get to top run speed from a standing speed
	public float deceleration = 4; //how fast we slow down on no input
	public float platformDropAnalogTolerance = 0.05f;

	public override void Awake()
	{
		if(Instance == null) Instance = this;
		base.Awake();

		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	//used for horizontal obstacle detection
	void OnControllerCollidedHorizontal (RaycastHit2D hit){}

	public override void LateUpdate()
	{
		if(combatState == CombatState.Aiming || combatState == CombatState.Blocking)
		{
			RotateBody();
		}
	}

	public override void Start()
	{
		_controller.onTriggerEnterEvent += OnTriggerEnterHandler;
		_controller.onTriggerExitEvent += OnTriggerExitHandler;

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
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode m)
	{
		if(scene.name == "Start")
		{
			canMove = true;
			canFace = true;
			_anim.SetTrigger("Reset");
			CameraController.Instance.RegisterMe(myTransform);
		}
	}

	void OnTriggerEnterHandler(Collider2D col)
	{
		if(col.CompareTag("Elevator") || col.CompareTag("Pickup"))
		{
			canJump = false;
		}

		//deflect fireballs if blocking
		if(col.transform.CompareTag("Fireball"))
		{
			if(combatState == CombatState.Blocking)
			{
				//change the direction of the fireball
				Fireball fireball = col.GetComponent<Fireball>();
				fireball.Velocity = Vector2.Reflect(fireball.Velocity, LookDirection);

				//slide
				OnHurt(fireball.myEntity);

				//make it offensive to enemies
				fireball.SetOffensiveToEnemies();
			}
		}
	}
	void OnTriggerExitHandler(Collider2D col)
	{
		if(col.CompareTag("Elevator") || col.CompareTag("Pickup"))
		{
			canJump = true;
		}
	}

	public override void OnRollEnter()
	{
		_controller.IgnoreEntityCollision();
		base.OnRollEnter();
	}

	public override void OnRollExit()
	{
		_controller.RestoreEntityCollision();
		base.OnRollExit();
	}

	public override void Subscribe()
	{
		ConversationManager.onConversationStarted += Stop;
		ConversationManager.onConversationEnded += Go;

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

		_controller.onControllerCollidedEventHorizontal += OnControllerCollidedHorizontal;

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

		blockAttackState.onEnter -= BlockAttackEnter;
		blockAttackState.onExit -= BlockAttackExit;

		_controller.onControllerCollidedEventHorizontal -= OnControllerCollidedHorizontal;

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
		SetDamping(actionDamping);
		canMove = false;
	}
	void BlockExit()
	{
		SetDamping(groundDamping);
		canMove = true;
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
		SetDamping(groundDamping);
		canMove = true;
		canFace = true;
	}

	void DropAttackEnter()
	{
		normalizedHorizontalSpeed = 0;
		canMove = false;
		canFace = false;
	}
	void DropAttackExit()
	{
		canMove = true;
		canFace = true;
	}

	public override void OnDisable()
	{
		//register methods to input events
		PlayerInput.onR1 -= Attack;
		PlayerInput.onRightTrigger -= StrongAttack;
		PlayerInput.onA -= Jump;
		PlayerInput.onB -= Roll;
		PlayerInput.onX -= BackStep;
		PlayerInput.onY -= _health.Heal;

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
		if(CanForce()) Force(slidingAttack);
		base.SlideAttack();
	}

	public override void OnHurt(Entity offender)
	{
		base.OnHurt(offender);

		//if blocking with the shield...
		if(combatState == CombatState.Blocking)
		{
			sparks.Emit(Random.Range(10,15));
			SFX.Instance.PlayFX("block", myTransform.position);
			_stamina.Drain(_stamina.blockDrain);
		}
		else
		{
			SFX.Instance.PlayFX("player_Hurt", myTransform.position);
			//start the flashing when hurt
			StartCoroutine(_flash.FlashSprites(_sprites, flashes, flashDuration, disable));
		}
	}

	public override void OnDeath ()
	{
		CameraController.Instance.UnRegisterMe(transform);
		HasDied();
		canMove = false;
		canFace = false;
		_velocity.x = 0;
		_anim.SetTrigger("Collapse");
		normalizedHorizontalSpeed = 0;
	}

	void Movement()
	{
		_controller.ignoreOneWayPlatformsThisFrame = PlayerInput.Instance.LAnalog.y < -platformDropAnalogTolerance;

		//if pushing right on the joystick...
		if(Mathf.Abs(PlayerInput.Instance.LAnalog.x) > 0.1f && canMove)
		{
			if(!ConversationManager.Instance.talking)
			{
				//accelerate if can move
				normalizedHorizontalSpeed += (PlayerInput.Instance.LAnalog.x < 0 ? -Time.deltaTime : Time.deltaTime)
				 * acceleration * Mathf.Abs(PlayerInput.Instance.LAnalog.x);

				if(facing == Facing.left){ normalizedHorizontalSpeed = Mathf.Clamp(normalizedHorizontalSpeed, -1, 0); }
				else { normalizedHorizontalSpeed = Mathf.Clamp(normalizedHorizontalSpeed, 0, 1); }
			}

			//set facing to analog direction
			if(Mathf.Abs(PlayerInput.Instance.LAnalog.x) > 0.1f)
			{
				SetFacing(PlayerInput.Instance.LAnalog.x > 0.1f ? Facing.right : Facing.left);
			}
		}

		/* //keyboard movement
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
			if(_controller == null) print("controller null");
			if(rollState == null) print("rollstate null");

			//only decelerate when the player is not performing any actions BUT CAN
			if(CanAct() || (!_controller.isGrounded && !rollState.inState && !backstepState.inState) || PauseMenu.paused)
			{
				if(Mathf.Abs(normalizedHorizontalSpeed) > 0.05f) 
					normalizedHorizontalSpeed += (normalizedHorizontalSpeed > 0 ? -Time.deltaTime : Time.deltaTime) * deceleration;
				else normalizedHorizontalSpeed = 0;

				if( _controller.isGrounded) _anim.SetInteger("AnimState", 0);
			}
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
		// grab our current _velocity to use as a base for all calculations
		_velocity = _controller.velocity;
		//input actions
		Actions();
	}

	void Actions()
	{
		if(!CanAct()) return;

		if(PlayerInput.Instance.L1Down)
		{
			if(_switcher.IsWeapon(Weapons.Bow))
			{
		  		Aim();
		  	}
		  	else if(_switcher.IsWeapon(Weapons.SwordShield))
		  	{
				Block();
		  	}
		}
		else { Idle(); }
	}

	public override void Idle()
	{
		base.Idle();
		SetSpeed (walkSpeed);
	}

	public bool CanAct()
	{
		if(_health.Dead) return false;
		if(!ready) return false;
		if(!_controller.isGrounded) return false;
		if(PauseMenu.paused) return false;

		return true;
	}

	public override void Block()
	{
		if(_stamina.CurrentStamina == 0) return;

		//set facing to analog direction
		if(Mathf.Abs(PlayerInput.Instance.RAnalog.x) > 0.1f)
		{
			SetFacing(PlayerInput.Instance.RAnalog.x > 0.1f ? Facing.right : Facing.left);
		}

		LookDirection = PlayerInput.Instance.RAnalog;
		SetSpeed (blockSpeed);

		base.Block();
	}

	public override void Aim()
	{
		//set facing to analog direction
		if(Mathf.Abs(PlayerInput.Instance.RAnalog.x) > 0.1f)
		{
			SetFacing(PlayerInput.Instance.RAnalog.x > 0.1f ? Facing.right : Facing.left);
		}

		LookDirection = PlayerInput.Instance.RAnalog;
		SetSpeed (blockSpeed);
		base.Aim();
	}

	public override void Jump()
	{
		if(!canJump) return;
		if(PlayerInput.Instance.L1Down) return;
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

		StartCoroutine(Ready(rollDelay));
		_anim.SetTrigger("Roll");
		SFX.Instance.PlayFX("jump", myTransform.position);
	}

	public override void Attack()
	{
		if(PauseMenu.paused) return;
		if(!_stamina.Ready) return;
		//can't attack without a weapon
		if(_switcher.IsWeapon(Weapons.Empty)) return;
		if(_switcher.IsWeapon(Weapons.Bow)) return;

		if(!ready) return;

		//falling attack
		if(_anim.GetBool("Falling") && height >= dropAttackHeight)// && !jumpState.inState)
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
		if(PlayerInput.Instance.L1Down) return;
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
