using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Player : Entity
{
	[Header("Movement Parms")]
	public bool moving = false;
	// movement config
	public float gravity = -25f;
	public float groundDamping = 20f; // how fast do we change direction? higher means faster
	public float inAirDamping = 5f;
	public float jumpHeight = 3f;
	public float force = 100;

	//backstepping
	public float maxHeight = 0.6f;
	public float maxDistance = 220f;

	[Header("Local References")]
	public WeaponController _switcher;

	private Vector3 _velocity;
	[HideInInspector] public UI _ui;
	[HideInInspector] public CameraController _cam;
	[HideInInspector] public LevelManager _manager;
	[HideInInspector] public PlayerInput _input;
	[HideInInspector] public ConversationManager _convoManager;

	[HideInInspector]
	public bool _ready = true;
	[HideInInspector]
	private bool transitioned;

	string currentSceneName;

	[Header("Player")]
	public float rollDelay = 0.65f;
	public float backStepDelay = 0.65f;
	public float jumpDelay = 0.3f;
	public float blockAttackDelay = 0.5f;

	[Header("Combo Counter")]
	public float timeLeft;
	[SerializeField] private int comboPoints = 0;

	//flashing
	private Flash _flash;
	[Header("Flash")]
	public int flashes = 6;
	public float flashDuration = 0.05f;
	public bool disable;
	private SpriteRenderer[] _sprites;

	LevelManager Manager {
		get { if(!_manager) _manager = GameObject.Find("_LevelManager").GetComponent<LevelManager>(); return _manager; }
	}

	public override void Awake()
	{
		base.Awake();
		SceneManager.sceneLoaded += OnSceneLoaded;

		_controller.onControllerCollidedEvent += OnControllerCollider;
		_controller.onTriggerEnterEvent += OnTriggerEnterEvent2D;
		_controller.onTriggerExitEvent += OnTriggerExitEvent2D;
	}

	void OnControllerCollider(RaycastHit2D hit)
	{
		//bail out of plain old ground hits, because they aren't that interesting
		if(hit.normal.y == 1) return;
		//
	}

	void OnTriggerEnterEvent2D(Collider2D col)
	{
		//Debug.Log("enter: " + col.gameObject.name);
	}

	void OnTriggerExitEvent2D(Collider2D col)
	{
		//Debug.Log("exit: " + col.gameObject.name);
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode m)
	{
		currentSceneName = scene.name;
	}

	public override void Start()
	{
		base.Start();
		_input = Manager.GetComponent<PlayerInput> ();
		//register methods to input events
		_input.onR1 += Attack;
		_input.onRightTrigger += StrongAttack;
		_input.onA += Jump;
		_input.onB += Roll;
		_input.onX += BackStep;
		_input.onY += _health.Heal;

		//delegate initialization
		SetSpeed(walkSpeed);
		
		//import components
		_convoManager = Manager.GetComponentInChildren<ConversationManager>();
		_flash = GetComponent<Flash>();
		_sprites = GetComponentsInChildren<SpriteRenderer>();
		_ui = _manager.transform.Find("UI").GetComponent<UI>();
		_cam = Camera.main.GetComponent<CameraController>();
		_cam.RegisterMe(myTransform);
	}

	public bool Moving()
	{
		if(normalizedHorizontalSpeed == 0)
			return false;
		else
			return true;
	}

	public bool SetFacing(bool left)
	{
		if(!left)
		{
			//setting this to vector2 will squash the 3d collider for blood particles
			transform.localScale = new Vector3( 1, transform.localScale.y, 1);
			FacingLeft = false;
		}
		else
		{
			transform.localScale = new Vector3( -1, transform.localScale.y, 1);
			FacingLeft = true;
		}

		return FacingLeft;
	}

	//triggers when blocking and hit by an attack
	public override void Slide()
	{
		base.Slide();
		_velocity.x = FacingLeft ? Mathf.Sqrt(maxDistance) : -Mathf.Sqrt(maxDistance);
		_controller.move(_velocity * Time.deltaTime);
	}

	public override void OnHurt()
	{
		if(_input._blocking) return;

		_velocity.y = Mathf.Sqrt(maxHeight * -gravity);
		_velocity.x = FacingLeft ? Mathf.Sqrt(maxDistance) : -Mathf.Sqrt(maxDistance);
		_controller.move(_velocity * Time.deltaTime);

		_sfx.PlayFX("player_Hurt", myTransform.position);

		//start the flashing when hurt
		StartCoroutine(_flash.FlashSprites(_sprites, flashes, flashDuration, disable));
	}

	public override void OnDeath ()
	{
		base.OnDeath ();
		_anim.SetInteger("AnimState", 2);
		normalizedHorizontalSpeed = 0;
	}

	void Update()
	{
		// grab our current _velocity to use as a base for all calculations
		_velocity = _controller.velocity;

		//dont move down if grounded
		if( _controller.isGrounded)
		{
			_velocity.y = 0;
		}

		//set moving bool
		moving = Moving();
		//animation speed controller
		_anim.SetFloat ("Speed", Mathf.Abs(_input.LAnalog.x));
		//make the animator change animations depending on how many times youve attacked
		_anim.SetFloat ("ComboPoints", Mathf.Clamp(comboPoints, 0, 3));
		//input actions
		Actions();
	}

	void Movement()
	{
		
		//reset
		normalizedHorizontalSpeed = 0;

		if(_convoManager.talking) return;

		_controller.ignoreOneWayPlatformsThisFrame = _input.LAnalog.y < -0.1f;

		//if pushing right on the joystick...
		if(_input.LAnalog.x > 0.1)
		{
			normalizedHorizontalSpeed = _input.LAnalog.x;
			SetFacing(false);
		}

		//else if pushing left on the joystick...
		else if(_input.LAnalog.x < -0.1)
		{
			normalizedHorizontalSpeed = _input.LAnalog.x;
			SetFacing(true);
		}

		//keyboard movement
		/*
		//else if pushing left on the keyboard
		else if(_input.LAnalog.x == -1) //if walking left
		{
			//animation speed keyboard
			_anim.SetFloat ("Speed", 1);
			normalizedHorizontalSpeed = -1;
			SetFacing(true);
			if( _controller.isGrounded)
				_anim.SetInteger("AnimState", 1);
		}

		//else if pushing right on the keyboard...
		else if(_input.LAnalog.x > 0) //if walking right
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
			normalizedHorizontalSpeed = 0;
			if( _controller.isGrounded) _anim.SetInteger("AnimState", 0);
		}
	}

	public bool CanAct()
	{
		if(!_ready) return false;
		if(!_controller.isGrounded) return false;
		if(_convoManager.talking) return false;
		if(_manager.paused) return false;
		return true;
	}

	void Actions()
	{
		if(!CanAct()) return;

		if(_input.L1Down) { Block(); }
		else { Idle(); }
	}

	void Idle()
	{
		SetSpeed (walkSpeed);
		combatState = CombatState.Idle;
	}
	public override void Block()
	{
		LookDirection = _input.RAnalog;
		combatState = CombatState.Blocking;
		SetSpeed (blockSpeed);
	}

	public void Jump()
	{
		if(!CanAct()) return;

		if(currentSceneName == "Menu") return;
		//cannot jump for x seconds
		//StartCoroutine(Ready(jumpDelay));
		_sfx.PlayFX("jump", myTransform.position);
		_velocity.y = Mathf.Sqrt( 2f * jumpHeight * -gravity);
		_controller.move(_velocity * Time.deltaTime);
	}

	public override void Roll()
	{
		if(!CanAct()) return;
		if(!_stamina.Ready) return;
		if(_switcher.IsWeapon(Weapons.Spear)) return;
		if(combatState == CombatState.Blocking) return;

		_stamina.UseStamina(rollDrain);
		combatState = CombatState.Rolling;
		_anim.SetTrigger("Roll");
		_sfx.PlayFX("jump", myTransform.position);
		
		//cannot roll for x seconds
		StartCoroutine(Ready(rollDelay));
		
		_velocity.y = Mathf.Sqrt(maxHeight * -gravity);
		_velocity.x = FacingLeft ? -Mathf.Sqrt(maxDistance) : Mathf.Sqrt(maxDistance);
		_controller.move(_velocity * Time.deltaTime);
	}

	void SlidingAttack()
	{
		_velocity.y = Mathf.Sqrt(maxHeight * -gravity);
		_velocity.x = -Mathf.Sqrt(maxDistance);
		_controller.move(_velocity * Time.deltaTime);
	}

	public override void Attack()
	{
		if(!CanAct()) return;
		if(!_stamina.Ready) return;
		//can't attack without a weapon
		if(_switcher.IsWeapon(Weapons.Empty)) return;
		if(_switcher.IsWeapon(Weapons.Bow)) return;

		//if moving but not blocking, don't attack
		if(Mathf.Abs(normalizedHorizontalSpeed) > .1f && combatState != CombatState.Blocking) return;

		//TODO:
		if(Mathf.Abs(normalizedHorizontalSpeed) > .8f && combatState != CombatState.Blocking)
		{
			SlidingAttack();
			//perform a sliding thrust if running at full speed!
		}

		if(combatState == CombatState.Blocking)
		{
			_anim.SetTrigger("BlockingAttack");
			if(blockAttackDelay > 0) StartCoroutine(Ready(blockAttackDelay));
		}
		else
		{
			combatState = CombatState.Attacking;
			_anim.SetTrigger("Attack");
			if(_switcher.weapon.delay > 0) StartCoroutine(Ready(_switcher.weapon.delay));
		}

		_stamina.UseStamina(lightAttackDrain);
		SetSpeed (blockSpeed);
	}

	void StrongAttack()
	{
		if(!_stamina.Ready) return;
		if(!CanAct()) return;
		//can't attack without a weapon
		if(_switcher.IsWeapon(Weapons.Empty)) return;
		if(Mathf.Abs(normalizedHorizontalSpeed) > .1f && combatState != CombatState.Blocking) return;

		_stamina.UseStamina(heavyAttackDrain);

		SetSpeed (blockSpeed);
		_anim.SetTrigger("StrongAttack");
		if(_switcher.weapon.delay > 0) StartCoroutine(Ready(_switcher.weapon.delay));
	}

	void BackStep()
	{
		if(!CanAct()) return;
		if(Mathf.Abs(normalizedHorizontalSpeed) > .1f) return;
		if(!_switcher.weapon.canBackStep) return;

		_stamina.UseStamina(backStepDrain);

		combatState = CombatState.BackStepping;
		//cannot roll for x seconds
		//StartCoroutine(Ready(backStepDelay));

		_anim.SetTrigger("BackStep");
		_sfx.PlayFX("jump", myTransform.position);

		if(FacingLeft)
		{
			_velocity.y = Mathf.Sqrt(maxHeight * -gravity);
			_velocity.x = Mathf.Sqrt(maxDistance);
		}
		else
		{
			_velocity.y = Mathf.Sqrt(maxHeight * -gravity);
			_velocity.x = -Mathf.Sqrt(maxDistance);
		}

		_controller.move(_velocity * Time.deltaTime);
	}

	//handle movement in fps time
	void FixedUpdate()
	{
		Movement();

		//Falling
		bool falling = !_controller.isGrounded && combatState != CombatState.Rolling && combatState != CombatState.BackStepping;
		if(falling) combatState = CombatState.Jumping;
		_anim.SetBool("Falling", falling);

		//foot dust
		if(_controller.isGrounded && Mathf.Abs(normalizedHorizontalSpeed) > 0.1f)
		{
			_footDust.Play();
		}
		else
		{
			_footDust.Stop();
		}

		//apply horizontal speed smoothing ------------------------------------------------------------------------------
		float smoothedMovementFactor = _controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
		_velocity.x = Mathf.Lerp( _velocity.x, normalizedHorizontalSpeed * speed, Time.deltaTime * smoothedMovementFactor );
		//apply gravity before moving
		_velocity.y += gravity * Time.deltaTime;
		_controller.move( _velocity * Time.deltaTime);
	}

	//combos
	public void AddComboPoint()
	{
		comboPoints ++;
	}

	//Stamina
	public IEnumerator Ready(float delay)
	{
		_ready = false;
		yield return new WaitForSeconds (delay);
		_ready = true;
	}

	//double tapping
	public float buttonCooler = 0.5f;
	private int buttonCount;

	void ArcadeControls()
	{
	/*
		//cooldown
		if ( buttonCooler > 0 )
			buttonCooler -= 1 * Time.deltaTime;
		else
			buttonCount = 0;

		//Arcade (Double Tap) Rolling
		if(_controller.isGrounded 
		&& _ready 
		&& !_switcher.IsWeapon(Weapons.Spear))
		{
			if (_input._leftOnce 
			|| _input._rightOnce 
			|| _input._roll)
			{
				if ( buttonCooler > 0 && buttonCount == 1 || _input._roll){ //Number of Taps you want Minus One
					//Has double tapped
					combatState = CombatState.Rolling;
					
					_anim.SetTrigger("Roll");
					_sfx.PlayFX("jump", myTransform.position);
					
					//cannot roll for x seconds
					StartCoroutine(Ready(rollDelay));
					
					if(!FacingLeft)
					{
						_velocity.y = Mathf.Sqrt(maxHeight * -gravity);
						_velocity.x = Mathf.Sqrt(maxDistance);
					}
					else
					{
						_velocity.y = Mathf.Sqrt(maxHeight * -gravity);
						_velocity.x = -Mathf.Sqrt(maxDistance);
					}
				}
				else
				{
					buttonCooler = 0.5f ; 
					buttonCount += 1 ;
				}
			}
		}
		*/
	}
}
