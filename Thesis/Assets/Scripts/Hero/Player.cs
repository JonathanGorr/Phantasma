using UnityEngine;
using System.Collections;

public class Player : Entity
{
	[Header("Movement Parms")]
	public bool moving = false;
	// movement config
	public float gravity = -25f;
	public float groundDamping = 20f; // how fast do we change direction? higher means faster
	public float inAirDamping = 5f;
	public float jumpHeight = 3f;

	//backstepping
	public float maxHeight = 0.6f;
	public float maxDistance = 220f;

	[Header("Local References")]
	public Health _health;
	public WeaponController _switcher;

	private Vector3 _velocity;
	[HideInInspector] public SFX _sfx;
	[HideInInspector] public CameraController _cam;
	[HideInInspector] public LevelManager _manager;
	[HideInInspector] public PlayerInput _input;
	[HideInInspector] public ConversationManager _convoManager;

	[HideInInspector]
	public bool _ready = true;
	[HideInInspector]

	private bool transitioned;

	[HideInInspector]
	public bool facingLeft = false;

	[Header("Player")]
	public float rollDelay = 0.65f;
	public float backStepDelay = 0.65f;
	public float jumpDelay = 0.3f;
	public float blockAttackDelay = 0.5f;

	//combocounter
	private int comboPoints = 0;
	public float timeLeft;

	//flashing
	private Flash _flash;
	public int flashes = 6;
	public float flashDuration = 0.05f;
	public bool disable;
	private SpriteRenderer[] _sprites;
	
	void Start()
	{
		//delegate initialization
		SetSpeed(walkSpeed);
		
		//import components
		_manager = GameObject.Find("_LevelManager").GetComponent<LevelManager>();
		_convoManager = _manager.GetComponentInChildren<ConversationManager>();
		_input = _manager.GetComponent<PlayerInput> ();
		_flash = GetComponent<Flash>();
		_sprites = GetComponentsInChildren<SpriteRenderer>();
		_sfx = _manager.GetComponent<SFX>();
		_cam = Camera.main.GetComponent<CameraController>();
		DontDestroyOnLoad(this.gameObject);

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

	public override void OnHurt()
	{
		//knockback
		if(!_input._blocking)
		{
			if(facingLeft)
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
		_anim.SetFloat ("Speed", Mathf.Abs(_input._axis));
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
		if(!_switcher.weapon.canMove) return;

		//if pushing right on the joystick...
		if(_input._axis > 0.1)
		{
			normalizedHorizontalSpeed = _input._axis;
			if(transform.localScale.x < 0f)
			{
				//setting this to vector2 will squash the 3d collider for blood particles
				transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, 1);
			}
			facingLeft = false;
		}

		//else if pushing left on the joystick...
		else if(_input._axis < -0.1)
		{
			normalizedHorizontalSpeed = _input._axis;
			if(transform.localScale.x > 0f)
			{
				transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, 1);
			}
			facingLeft = true;
		}

		//keyboard movement
		//else if pushing left on the keyboard
		else if(_input._left) //if walking left
		{
			//animation speed keyboard
			_anim.SetFloat ("Speed", 1);

			normalizedHorizontalSpeed = -1;

			//flipping
			if( transform.localScale.x > 0f)
			{
				transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );
				facingLeft = true;
			}
			
			if( _controller.isGrounded)
				_anim.SetInteger("AnimState", 1);

		}

		//else if pushing right on the keyboard...
		else if(_input._right) //if walking right
		{
			//animation speed keyboard
			_anim.SetFloat ("Speed", 1);

			normalizedHorizontalSpeed = 1;

			if( transform.localScale.x < 0f)
			{
				transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );
				facingLeft = false;
			}
			
			if( _controller.isGrounded)
				_anim.SetInteger("AnimState", 1);
		}

		//else if no input, stand still...
		else
		{
			normalizedHorizontalSpeed = 0;
			
			if( _controller.isGrounded)
				_anim.SetInteger("AnimState", 0);
		}
	}

	void Actions()
	{
		if(!_ready) return;
		if(!_controller.isGrounded) return;
		if(_convoManager.talking) return;

		//if roll pressed and can roll...
		if(_input._roll && _switcher.weapon.canRoll){ Roll(); }
		else if(_input._attack){ Attack(); }
		else if(_input._backStep && _switcher.weapon.canBackStep){ BackStep(); }
		else if(_input._strongAttack){ StrongAttack(); }
		else if(_input._blocking && _switcher.weapon.canBlock) { Block(); }
		else if(_input._jump){ Jump(); }
		else { Idle(); }
	}

	void Idle()
	{
		SetSpeed (walkSpeed);
		combatState = CombatState.Idle;
	}

	void Jump()
	{
		combatState = CombatState.Jumping;
		//cannot jump for x seconds
		StartCoroutine(Ready(jumpDelay));
		_sfx.PlayFX("jump", myTransform.position);
		_velocity.y = Mathf.Sqrt( 2f * jumpHeight * -gravity);
	}

	void Block()
	{
		combatState = CombatState.Blocking;
		SetSpeed (blockSpeed);
	}

	void Roll()
	{
		if(combatState == CombatState.Blocking) return;

		combatState = CombatState.Rolling;
		_anim.SetTrigger("Roll");
		_sfx.PlayFX("jump", myTransform.position);
		
		//cannot roll for x seconds
		StartCoroutine(Ready(rollDelay));
		
		if(!facingLeft)
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

	void Attack()
	{
		//if moving but not blocking, don't attack
		if(Mathf.Abs(normalizedHorizontalSpeed) > .1f && combatState != CombatState.Blocking) return;

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
		SetSpeed (blockSpeed);
	}

	void StrongAttack()
	{
		if(Mathf.Abs(normalizedHorizontalSpeed) > .1f && combatState != CombatState.Blocking) return;
		if(_input._jump) return;

		SetSpeed (blockSpeed);
		_anim.SetTrigger("StrongAttack");
		if(_switcher.weapon.delay > 0) StartCoroutine(Ready(_switcher.weapon.delay));
	}

	void BackStep()
	{
		combatState = CombatState.BackStepping;
		//cannot roll for x seconds
		StartCoroutine(Ready(backStepDelay));

		_anim.SetTrigger("BackStep");
		_sfx.PlayFX("jump", myTransform.position);
		
		if(facingLeft)
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

	//handle movement in fps time
	void FixedUpdate()
	{
		Movement();

		//Falling
		_anim.SetBool("Falling", !_controller.isGrounded && combatState != CombatState.Rolling && combatState != CombatState.BackStepping);

		//foot dust
		if(_controller.isGrounded && Mathf.Abs(normalizedHorizontalSpeed) < 0.1f)
		{
			_footDust.Play();
		}

		// apply horizontal speed smoothing ------------------------------------------------------------------------------
		var smoothedMovementFactor = _controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
		_velocity.x = Mathf.Lerp( _velocity.x, normalizedHorizontalSpeed * speed, Time.deltaTime * smoothedMovementFactor );
		
		// apply gravity before moving
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
				if ( buttonCooler > 0 && buttonCount == 1 || _input._roll/*Number of Taps you want Minus One*/){
					//Has double tapped
					combatState = CombatState.Rolling;
					
					_anim.SetTrigger("Roll");
					_sfx.PlayFX("jump", myTransform.position);
					
					//cannot roll for x seconds
					StartCoroutine(Ready(rollDelay));
					
					if(!facingLeft)
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
	}
}
