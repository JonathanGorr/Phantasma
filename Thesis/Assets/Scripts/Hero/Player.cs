using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// The player class derives from the entity base class and is responsible for controlling the main Player.
/// It also contains methods used to perform many actions including attacking, jumping backstepping and moving.
/// </summary>
public class Player : Entity
{
	public static Player Instance = null;					//there will only be 1 player( currently )
	public ParticleSystem sparks;							//sparks that emit from shield when hit//TODO: add shield script that contains this functionality and listens to health on hurt event

	[Header("Local References")]
	public WeaponController _switcher;						//our weapon switch controller
	private SlideAttackStateBehaviour slideAttackState;		//slide attack animation state
	private DropAttackStateBehaviour dropAttackState;		//drop attack animation state
	private BlockAttackStateBehaviour blockAttackState;		//block attack animation state

	//flashing
	//TODO: replace flashing with shader and separate script
	[Header("Flash")]
	public int flashes = 6;									//how many times our sprite flashes on damage taken
	private Flash _flash;									//handles the flashing effect
	public float flashDuration = 0.05f;						//how long each flash lasts
	public bool disable;									//disable flashing?
	private SpriteRenderer[] _sprites;						//the sprites we would like to flash

	[Header("Movement")]
	public float acceleration = 2; 							//how fast we get to top run speed from a standing speed
	public float deceleration = 4; 							//how fast we slow down on no input
	public float platformDropAnalogTolerance = 0.05f;		//how sensitive our dropping input detection is

	public override void Awake()
	{
		//establish our singleton once on spawn
		if(Instance == null) Instance = this;
		base.Awake();
		//listen to scene changes
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	//used for horizontal obstacle detection
	void OnControllerCollidedHorizontal (RaycastHit2D hit){}

	public override void Update()
	{
		base.Update();
		// grab our current _velocity to use as a base for all calculations
		_velocity = _controller.velocity;
		//player actions
		Actions();
	}

	public override void FixedUpdate()
	{
		base.FixedUpdate();
		//handle movement
		Movement();
	}

	public override void LateUpdate()
	{
		//rotate our body if blocking or aiming
		if(combatState == CombatState.Aiming 
		|| combatState == CombatState.Blocking)
		{
			RotateBody();
		}
	}

	public override void Start()
	{
		_controller.onTriggerEnterEvent += OnTriggerEnterHandler;
		_controller.onTriggerExitEvent += OnTriggerExitHandler;

		base.Start();
		//subscribe
		SubscribeInput();
		//delegate initialization
		SetSpeed(walkSpeed);
		SetDamping(groundDamping);
		
		//import components
		_flash = GetComponent<Flash>();
		_sprites = GetComponentsInChildren<SpriteRenderer>();
	}

	/// <summary>
	/// Subscribe action methods to buttons in the PlayerInput class.
	/// </summary>
	void SubscribeInput()
	{
		//register methods to input events
		PlayerInput.onR1 += Attack;
		PlayerInput.onRightTrigger += StrongAttack;
		PlayerInput.onA += Jump;
		PlayerInput.onB += Roll;
		PlayerInput.onX += BackStep;
		PlayerInput.onY += _health.Heal;

		//register keyboad input
		PlayerInput.onJump += Jump;
	}

	/// <summary>
	/// Unsubscribe all action methods to PlayerInput.
	/// </summary>
	void UnsubInput()
	{
		//register methods to input events
		PlayerInput.onR1 -= Attack;
		PlayerInput.onRightTrigger -= StrongAttack;
		PlayerInput.onA -= Jump;
		PlayerInput.onB -= Roll;
		PlayerInput.onX -= BackStep;
		PlayerInput.onY -= _health.Heal;

		//register keyboad input
		PlayerInput.onJump -= Jump;
	}

	public override void OnDisable()
	{
		UnsubInput();
		base.OnDisable();
	}

	/// <summary>
	/// Called each time a new scene is loaded.
	/// </summary>
	/// <param name="scene">Scene.</param>
	/// <param name="m">M.</param>
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

	/// <summary>
	/// Called whenever our charactercontroller2d ENTERS a trigger.
	/// </summary>
	/// <param name="col">Col.</param>
	void OnTriggerEnterHandler(Collider2D col)
	{
		//cannot jump on the elevator or when standing over a pickup object
		if(col.CompareTag("Elevator") 
		|| col.CompareTag("Pickup"))
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
				//slide backwards slightly
				OnHurt(fireball.myEntity);
				//make it offensive to enemies
				fireball.SetOffensiveToEnemies();
			}
		}
	}

	/// <summary>
	/// Called whenever our charactercontroller2d EXITS a trigger.
	/// </summary>
	/// <param name="col">Col.</param>
	void OnTriggerExitHandler(Collider2D col)
	{
		if(col.CompareTag("Elevator") || col.CompareTag("Pickup"))
		{
			canJump = true;
		}
	}

	/// <summary>
	/// Called when we ENTER our roll animation.
	/// </summary>
	public override void OnRollEnter()
	{
		_controller.IgnoreEntityCollision();
		base.OnRollEnter();
	}

	/// <summary>
	/// Called when we EXIT our roll animation.
	/// </summary>
	public override void OnRollExit()
	{
		_controller.RestoreEntityCollision();
		base.OnRollExit();
	}

	/// <summary>
	/// Handles the subscribing to all animation behaviours and other types of events found on our attached animator.
	/// </summary>
	public override void Subscribe()
	{
		//stop the movement of our controller if a conversation has started
		ConversationManager.onConversationStarted += Stop;
		//allow movement again at the end of a conversation
		ConversationManager.onConversationEnded += Go;

		//register our enter and exit methods to our slide attack behaviour
		slideAttackState = _anim.GetBehaviour<SlideAttackStateBehaviour>();
		slideAttackState.onEnter += SlideAttackEnter;
		slideAttackState.onExit += SlideAttackExit;
		animMethods.onSlide += SlideAttack;

		//register our enter and exit methods to our dropattack behaviour
		dropAttackState = _anim.GetBehaviour<DropAttackStateBehaviour>();
		dropAttackState.onEnter += DropAttackEnter;
		dropAttackState.onExit += DropAttackExit;

		//register our enter and exit methods to our block attack behaviour
		blockAttackState = _anim.GetBehaviour<BlockAttackStateBehaviour>();
		blockAttackState.onEnter += BlockAttackEnter;
		blockAttackState.onExit += BlockAttackExit;

		//listen to our controller collision event
		_controller.onControllerCollidedEventHorizontal += OnControllerCollidedHorizontal;

		//subscribe to all things inherited in our base class
		base.Subscribe();
	}

	/// <summary>
	/// Handles the unsubscribing from all animation behaviours and other types of events found on our attached animator.
	/// </summary>
	public override void UnSubscribe()
	{
		//unsub from conversation start and stop
		ConversationManager.onConversationStarted -= Stop;
		ConversationManager.onConversationEnded -= Go;

		//unsub methods enter and exit from slide attack animation behaviour
		slideAttackState.onEnter -= SlideAttackEnter;
		slideAttackState.onExit -= SlideAttackExit;
		animMethods.onSlide -= SlideAttack;

		//unsub methods enter and exit from drop attack animation behaviour
		dropAttackState.onEnter -= DropAttackEnter;
		dropAttackState.onExit -= DropAttackExit;

		//unsub methods enter and exit from block attack animation behaviour
		blockAttackState.onEnter -= BlockAttackEnter;
		blockAttackState.onExit -= BlockAttackExit;

		//don't listen to our controller collision event
		_controller.onControllerCollidedEventHorizontal -= OnControllerCollidedHorizontal;

		//unsub from inherited events and behaviours
		base.UnSubscribe();
	}

	/// <summary>
	/// Stop and prevent movement.
	/// </summary>
	void Stop()
	{
		normalizedHorizontalSpeed = 0;
		_anim.SetFloat("Speed", 0);
		canMove = false;
		canFace = false;
		_velocity.x = 0;
	}

	/// <summary>
	/// Allow movement.
	/// </summary>
	void Go()
	{
		canMove = true;
		canFace = true;
	}

	/// <summary>
	/// Block animation has been ENTERED.
	/// </summary>
	void BlockEnter()
	{
		SetDamping(actionDamping);
		canMove = false;
	}

	/// <summary>
	/// Block animation has been EXITED.
	/// </summary>
	void BlockExit()
	{
		SetDamping(groundDamping);
		canMove = true;
	}

	/// <summary>
	/// Block attack animation has been entered.
	/// </summary>
	void BlockAttackEnter()
	{
		SetDamping(actionDamping);
		canMove = false;
		canFace = false;
		StartCoroutine(Ready(blockAttackDelay));
	}

	/// <summary>
	/// Blocks the attack exit.
	/// </summary>
	void BlockAttackExit()
	{
		SetDamping(groundDamping);
		canMove = true;
		canFace = true;
	}

	/// <summary>
	/// Drop attack animation entered.
	/// </summary>
	void DropAttackEnter()
	{
		normalizedHorizontalSpeed = 0;
		canMove = false;
		canFace = false;
	}

	/// <summary>
	/// Drop attack animation exited.
	/// </summary>
	void DropAttackExit()
	{
		canMove = true;
		canFace = true;
	}

	/// <summary>
	/// Slide attack animation state entered.
	/// </summary>
	public void SlideAttackEnter()
	{
		SetDamping(actionDamping);
		canMove = false;
		canFace = false;
	}

	/// <summary>
	/// Slide attack animation state exited.
	/// </summary>
	public void SlideAttackExit()
	{
		SetDamping(groundDamping);
		canMove = true;
		canFace = true;
	}

	/// <summary>
	/// Perform a slide attack.
	/// </summary>
	public override void SlideAttack()
	{
		if(CanForce()) Force(slidingAttack);
		base.SlideAttack();
	}

	/// <summary>
	/// Actions related to taking damage.
	/// Called from my health class by enemy weapons and other kinds of damagers.
	/// </summary>
	/// <param name="offender">Offender.</param>
	public override void OnHurt(Entity offender)
	{
		//inherit instructions from base class for this offender
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

	/// <summary>
	/// Instructions associated with dying.
	/// </summary>
	public override void OnDeath ()
	{
		CameraController.Instance.UnRegisterMe(transform);
		HasDied();
		Stop();
		_anim.SetTrigger("Collapse");
		normalizedHorizontalSpeed = 0;
	}

	/// <summary>
	/// Listens for controller input ( LAnalog ) to move this player controller with.
	/// </summary>
	/// <returns><c>true</c>, If left analog stick is moved, <c>false</c> otherwise.</returns>
	bool ControllerMovement()
	{
		//if pushing right on the joystick...
		if(Mathf.Abs(PlayerInput.Instance.LAnalog.x) > 0.1f && canMove)
		{
			//move if not in a conversation with an npc
			if(!ConversationManager.Instance.talking)
			{
				//accelerate left or right
				normalizedHorizontalSpeed += ( PlayerInput.Instance.LAnalog.x < 0 ? -Time.deltaTime : Time.deltaTime)
				 * acceleration * Mathf.Abs(PlayerInput.Instance.LAnalog.x);

				//clamp horizontal speed to -1-0 if facing left; 0-1 if facing right
				normalizedHorizontalSpeed = 
				Mathf.Clamp(normalizedHorizontalSpeed, //current
				(facing == Facing.left ? -1.0f : 0.0f), //min
				(facing == Facing.right ? 1.0f : 0.0f)); //max
			}
			//set facing to analog direction
			Facing = PlayerInput.Instance.LAnalog.x > 0.1f ? Facing.right : Facing.left;
			//input
			return true;
		}
		//no input
		return false;
	}

	/// <summary>
	/// Moves the controller if key presses (A or D) are detected.
	/// </summary>
	/// <returns><c>true</c>, if mouse movement was keyboarded, <c>false</c> otherwise.</returns>
	bool KeyboardMouseMovement()
	{
		//if pushing left on the keyboard
		if(PlayerInput.KeyboardLeft) //if walking left
		{
			//animation speed keyboard
			_anim.SetFloat ("Speed", 1);
			normalizedHorizontalSpeed = -1;
			Facing = Facing.left;
			if( _controller.isGrounded) _anim.SetInteger("AnimState", 1);

			//input
			return true;
		}
		//else if pushing right on the keyboard...
		else if(PlayerInput.KeyboardRight) //if walking right
		{
			//animation speed keyboard
			_anim.SetFloat ("Speed", 1);
			normalizedHorizontalSpeed = 1;
			Facing = Facing.right;
			if( _controller.isGrounded) _anim.SetInteger("AnimState", 1);

			//input
			return true;
		}

		//no input
		return false;
	}

	/// <summary>
	/// Handle no input from any controller or peripheral whatsoever.
	/// Decelerate movement.
	/// </summary>
	void NoInput()
	{
		//only decelerate when the player is not performing any actions BUT CAN
		if(CanAct() || (!_controller.isGrounded && !rollState.inState && !backstepState.inState) || PauseMenu.paused)
		{
			if(Mathf.Abs(normalizedHorizontalSpeed) > 0.05f) 
				normalizedHorizontalSpeed += (normalizedHorizontalSpeed > 0 ? -Time.deltaTime : Time.deltaTime) * deceleration;
			else normalizedHorizontalSpeed = 0;

			if( _controller.isGrounded) _anim.SetInteger("AnimState", 0);
		}
	}

	/// <summary>
	/// Handle movement.
	/// </summary>
	void Movement()
	{
		//ignore one way platforms if we are pushing down the left analog stick; drop
		_controller.ignoreOneWayPlatformsThisFrame = PlayerInput.Instance.LAnalog.y < -platformDropAnalogTolerance;
		//test input
		if(ControllerMovement() || KeyboardMouseMovement()) {}
		else NoInput();	//no input; decelerate to stand still...
		//set animation speed parameter
		_anim.SetFloat("Speed", Mathf.Abs(normalizedHorizontalSpeed));
		//apply horizontal speed smoothing ------------------------------------------------------------------------------
		_velocity.x = Mathf.Lerp( _velocity.x, normalizedHorizontalSpeed * speed, Time.deltaTime * damping );
		//apply gravity before moving
		_velocity.y += gravity * Time.deltaTime;
		_controller.move( _velocity * Time.deltaTime);
	}

	/// <summary>
	/// Handle some actions performed by player.
	/// </summary>
	void Actions()
	{
		//check if can act
		if(!CanAct()) return;
		//test this every frame //TODO: replace with events that update on mouse up
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

	/// <summary>
	/// Handle the idle combat state.
	/// </summary>
	public override void Idle()
	{
		base.Idle();
		SetSpeed (walkSpeed);
	}

	/// <summary>
	/// Allows actions to be performed under certain conditions.
	/// </summary>
	/// <returns><c>true</c> if not dead, ready, grounded and not paused; otherwise, <c>false</c>.</returns>
	public bool CanAct()
	{
		if(_health.Dead) return false;				//if we are not dead
		if(!ready) return false;					//if we are ready
		if(!_controller.isGrounded) return false;	//if we are grounded
		if(PauseMenu.paused) return false;			//if we are not paused
		//can act
		return true;
	}

	/// <summary>
	/// The block action.
	/// </summary>
	public override void Block()
	{
		//don't block if we are out of stamina
		if(_stamina.CurrentStamina == 0) return;
		//allow the rotation of the body sprite when blocking
		Aim();
		//set look direction for body sprite rotation to the direction of the right analog stick
		LookDirection = PlayerInput.Instance.RAnalog;
		//set movement speed to that of block speed
		SetSpeed (blockSpeed);
		//inherit base class instructions
		base.Block();
	}

	/// <summary>
	/// Perform the aim action.
	/// </summary>
	public override void Aim()
	{
		//set facing to analog direction
		if(Mathf.Abs(PlayerInput.Instance.RAnalog.x) > 0.1f)
		{
			//set facing to rotation of 
			Facing = PlayerInput.Instance.RAnalog.x > 0.1f ? Facing.right : Facing.left;
		}
		//set the look direction to the right analog stick direction
		LookDirection = PlayerInput.Instance.RAnalog;
		//set the movement speed to that of block speed
		SetSpeed (blockSpeed);
		//inherit aim instructions from base class
		base.Aim();
	}

	/// <summary>
	/// Performs the jump action.
	/// </summary>
	public override void Jump()
	{
		if(!canJump) return;									//don't jump if turned off
		if(PlayerInput.Instance.L1Down) return;					//don't jump if aiming or blocking //TODO: check blocking or aiming combat state instead
		if(ConversationManager.Instance.talking) return;		//don't jump if talking
		if(SceneManager.GetActiveScene().name == "Menu") return;//don't jump if in menu
		if(!CanAct()) return;									//don't jump if can't act
		//play jump sound effect
		SFX.Instance.PlayFX("jump", myTransform.position);
		//inherit jump instructions from base class
		base.Jump();
	}

	/// <summary>
	/// Performs the 
	/// </summary>
	public override void Roll()
	{
		if(!CanAct()) return;									//don't roll if can't act
		if(!_stamina.Ready) return;								//don't roll if out of stamina
		if(!_switcher.weapon.canRoll) return;					//don't roll if equipped weapon won't allow it
		if(combatState == CombatState.Blocking) return;			//don't roll if blocking
		//perform inherited instructions
		base.Roll();
		//add a roll delay
		StartCoroutine(Ready(rollDelay));
		//play the roll animation
		_anim.SetTrigger("Roll");
		//play a jump sound effect
		SFX.Instance.PlayFX("jump", myTransform.position);
	}

	/// <summary>
	/// Performs an attack.
	/// </summary>
	public override void Attack()
	{
		if(PauseMenu.paused) return;							//don't attack if paused
		if(!_stamina.Ready) return;								//don't attack if out of stamina
		if(_switcher.IsWeapon(Weapons.Empty)) return;			//don't attack if there's no weapon equipped
		if(_switcher.IsWeapon(Weapons.Bow)) return;				//don't attack if there's a bow equipped
		if(!ready) return;										//don't attack if not ready

		//falling attack
		if(_anim.GetBool("Falling") //we are falling
		&& height >= dropAttackHeight) //we are high enough from the ground to fall
		{
			//play attack animation
			_anim.SetTrigger("Attack");
			//do not proceed
			return;
		}

		//we can only perform these attacks when grounded
		if(!_controller.isGrounded) return;

		//slide attack
		if(Mathf.Abs(normalizedHorizontalSpeed) >= .95f //are we running at ~max speed?
		&& combatState != CombatState.Blocking) //are we not blocking?
		{
			//trigger sliding attack animation
			_anim.SetTrigger("SlidingAttack");
			return;
		}

		base.Attack();

		//perform blocking attack if blocking
		if(combatState == CombatState.Blocking)
		{
			_anim.SetTrigger("BlockingAttack");
			//do not proceed
			return;
		}

		//perform regular attack
		combatState = CombatState.Attacking;
		_anim.SetTrigger("Attack");
		//set movement speed to block speed
		SetSpeed (blockSpeed);
	}

	/// <summary>
	/// Performs a strong attack.
	/// </summary>
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

	/// <summary>
	/// Performs a backstep.
	/// </summary>
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
}
