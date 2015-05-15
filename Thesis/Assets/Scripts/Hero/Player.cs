﻿using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	// movement config
	public float gravity = -25f;
	public float groundDamping = 20f; // how fast do we change direction? higher means faster
	public float inAirDamping = 5f;
	public float jumpHeight = 3f;

	//backstepping
	public float maxHeight = 0.6f;
	public float maxDistance = 220f;

	[HideInInspector]
	public float normalizedHorizontalSpeed = 0;
	
	private CharacterController2D _controller;
	private RaycastHit2D _lastControllerColliderHit;
	private Vector3 _velocity;
	private Health _health;
	private SFX _sfx;
	private ParticleSystem _footDust;
	private GameObject _managerGO;
	private LevelManager _manager;
	private PlayerInput _input;
	private ConversationManager _convoManager;

	//switch animator controller for each weapon
	Animator _animator;
	//overrided
	RuntimeAnimatorController runtimeAnimController;
	//overriders
	public AnimatorOverrideController bareFistedOverride;
	public AnimatorOverrideController swordOverride;
	public AnimatorOverrideController spearOverride;
	public AnimatorOverrideController bowOverride;

	[HideInInspector]
	public bool
		_ready = true,
		_rolling;

	private bool transitioned;

	[HideInInspector] public bool moving;

	[HideInInspector]
	public bool facingLeft = false;

	private float resultSpeed;

	//public variables
	public float 
		walkSpeed = 3f,
		runSpeed = 6f,
		runSpeedHeavy = 4f,
		blockSpeed = 0f,
		rollDelay = 0.65f,
		backStepDelay = 0.65f,
		jumpDelay = 0.3f,
		quickAttackDelay = 0.5f,
		blockAttackDelay = 0.5f,
		strongAttackDelay = 0.5f,
		quickShotDelay = 1f;

	//double tapping
	public float buttonCooler = 0.5f;
	private int buttonCount;

	//combocounter
	private int comboPoints = 0;
	public float timeLeft;

	//flashing
	private Flash _flash;
	public int flashes = 6;
	public float flashDuration = 0.05f;
	public bool disable;
	private SpriteRenderer[] _sprites;
	
	private WeaponSwitcher _switcher;

	//control speed
	delegate void Speedometer(float num);
	Speedometer speed;

	void Awake()
	{
		//delegate initialization
		speed = Speed;
		speed(0);
		
		//import components
		_managerGO = GameObject.Find("_LevelManager");
		_manager = _managerGO.GetComponent<LevelManager>();
		_convoManager = _manager.GetComponentInChildren<ConversationManager>();
		_input = _manager.GetComponent<PlayerInput> ();
		_health = GetComponent<Health>();
		_animator = GetComponent<Animator>();
		_controller = GetComponent<CharacterController2D>();
		_flash = GetComponent<Flash>();
		_sprites = GetComponentsInChildren<SpriteRenderer>();
		_switcher = _manager.GetComponent<WeaponSwitcher> ();
		_sfx = GetComponent<SFX> ();
		_footDust = GetComponentInChildren<ParticleSystem>();

		// listen to some events for illustration purposes
		_controller.onControllerCollidedEvent += onControllerCollider;
		_controller.onTriggerEnterEvent += onTriggerEnterEvent;
		_controller.onTriggerExitEvent += onTriggerExitEvent;
	}

	#region Event Listeners

	void onControllerCollider( RaycastHit2D hit )
	{
		// logs any collider hits if uncommented. it gets noisy so it is commented out for the demo
		//Debug.Log( "flags: " + _controller.collisionState + ", hit.normal: " + hit.normal );
	}
	
	void onTriggerEnterEvent( Collider2D col )
	{
		//Debug.Log( "onTriggerEnterEvent: " + col.gameObject.name );
	}
	
	void onTriggerExitEvent( Collider2D col )
	{
		//Debug.Log( "onTriggerExitEvent: " + col.gameObject.name );
	}

	#endregion

	void Update()
	{
		if(normalizedHorizontalSpeed > 0)
			moving = true;
		else
			moving = false;
	}

	//handle movement in fps time
	void FixedUpdate()
	{
		//animation speed controller
		_animator.SetFloat ("Speed", Mathf.Abs(_input._axis));

		//make the animator change animations depending on how many times youve attacked
		_animator.SetFloat ("ComboPoints", Mathf.Clamp(comboPoints, 0, 3));

		//switch animators

		switch(_switcher.currentWeapon)
		{
		//shortsword
		case 1:
			//print ("Shortsword(1) is equipped");
			_animator.runtimeAnimatorController = swordOverride;
			break;
		case 2:
			//print ("Spear(2) is equipped");
			_animator.runtimeAnimatorController = spearOverride;
			break;
		case 3:
			//print ("Bow(3) is equipped");
			_animator.runtimeAnimatorController = bowOverride;
			break;
		case 0:
			//print ("Nothing(0) is equipped");
			_animator.runtimeAnimatorController = bareFistedOverride;
			break;
		}

		// grab our current _velocity to use as a base for all calculations
		_velocity = _controller.velocity;

		//dont move down if grounded
		if( _controller.isGrounded)
		{
			_velocity.y = 0;
		}

		//if not aiming...
		if(!_input._aiming)
		{
			//axis flipping and movement ----------------------------------------------
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

			//keyboard movement------------------------------------------------------

			//else if pushing left on the keyboard
			else if(_input._left) //if walking left
			{
				//animation speed keyboard
				_animator.SetFloat ("Speed", 1);

				normalizedHorizontalSpeed = -1;

				//flipping
				if( transform.localScale.x > 0f)
				{
					transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );
					facingLeft = true;
				}
				
				if( _controller.isGrounded)
					_animator.SetInteger("AnimState", 1);

			}

			//else if pushing right on the keyboard...
			else if(_input._right) //if walking right
			{
				//animation speed keyboard
				_animator.SetFloat ("Speed", 1);

				normalizedHorizontalSpeed = 1;

				if( transform.localScale.x < 0f)
				{
					transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );
					facingLeft = false;
				}
				
				if( _controller.isGrounded)
					_animator.SetInteger("AnimState", 1);
			}

			//else if no input, stand still...
			else
			{
				normalizedHorizontalSpeed = 0;
				
				if( _controller.isGrounded)
					_animator.SetInteger("AnimState", 0);
			}
		}

		//else if aiming, cannot move, but can flip...
		else
		{
			normalizedHorizontalSpeed = 0;
		}

		//Jump-------------------------------------------------------------
		//We can only jump whilst grounded
		if(_controller.isGrounded 		//if grounded
		   && _input._jump 				//if jump key is pressed
		   && !_input._aiming 			//if not aiming
	 	   && !_input._blocking 		//if not blocking
		   && !_input._strongAttack) 	//if not attacking
		{
			//cannot jump for x seconds
			StartCoroutine(Ready(jumpDelay));

			_sfx.JumpSound();
			_velocity.y = Mathf.Sqrt( 2f * jumpHeight * -gravity);
		}

		//Falling
		//If not grounded; falling ---------------------------------------------------
		if(!_controller.isGrounded 
		   && !_rolling)
		{
			_animator.SetBool("Falling", true);
		}
		else
			_animator.SetBool("Falling", false);

		//Backstepping ------------------------------------------------------------------
		if(_input._backStep 					//if backstep button is pressed
		   && _controller.isGrounded 			//if grounded
		   && _ready 							//if ready
		   && normalizedHorizontalSpeed == 0) 	//if not moving
		{
			//cannot roll for x seconds
			StartCoroutine(Ready(backStepDelay));

			_animator.SetTrigger("BackStep");
			_sfx.JumpSound();
			
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

		//Arcade (Double Tap) Rolling -------------------------------------------------
		if(_controller.isGrounded && _ready && _switcher.currentWeapon != 2)
		{
			if (_input._leftOnce || _input._rightOnce || _input._roll)
			{
				if ( buttonCooler > 0 && buttonCount == 1 || _input._roll/*Number of Taps you want Minus One*/){
					//Has double tapped
					_rolling = true;
					
					_animator.SetTrigger("Roll");
					_sfx.JumpSound();
					
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
			else
				_rolling = false;
		}

		//cooldown
		if ( buttonCooler > 0 )
			buttonCooler -= 1 * Time.deltaTime ;
		else
			buttonCount = 0;

		//knockback on player hurt -------------------------------------------
		if(_health.playerHurt)
		{
			if(!_input._blocking)
			{
				if(facingLeft){
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

			//move player back
			_health.playerHurt = false;
		}
		
		// apply horizontal speed smoothing ------------------------------------------------------------------------------
		var smoothedMovementFactor = _controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
		_velocity.x = Mathf.Lerp( _velocity.x, normalizedHorizontalSpeed * resultSpeed, Time.fixedDeltaTime * smoothedMovementFactor );
		
		// apply gravity before moving
		_velocity.y += gravity * Time.fixedDeltaTime;
		_controller.move( _velocity * Time.fixedDeltaTime);
		
		// reset input
		_input._jump = false;

		//Blocking -----------------------------------------------------
		if(_switcher.currentWeapon == 1)
		{
			if(_input._blocking)
				_animator.SetBool("Blocking", true);
			else
				_animator.SetBool("Blocking", false);
		}

		//Blocking and Strong Attack Speed--------------------------------
		if(_controller.isGrounded)
		{
			if(_input._blocking)
				Speed (blockSpeed);
			else if(_input._strongAttack || _input._attack)
				Speed (blockSpeed);
			else
				Speed (walkSpeed);
		}
		
		//strong attack --------------------------------------------------
		if(_controller.isGrounded)
		{
			if(_input._strongAttack 	//if strong attack button pressed
			   && !_input._jump 		//if not jumping
			   && !_input._blocking 	//if not blocking
			   && _ready)
			{
				_animator.SetTrigger("StrongAttack");
				if(strongAttackDelay > 0) StartCoroutine(Ready(strongAttackDelay));
			}
		}
		
		//Quick Attack----------------------------------------------------------
		if(_input._attack 						//if attack button pressed
		   && !_input._blocking 				//if not blocking
		   && normalizedHorizontalSpeed == 0 	//if not moving
		   && _controller.isGrounded			//if grounded
		   && _ready							//if ready
		   && !_convoManager.talking) 			//if grounded
		{
			//if the bow is not equipped, just attack
			if(_switcher.currentWeapon != 3)
			{
				_animator.SetTrigger("Attack");
				if(quickAttackDelay > 0) StartCoroutine(Ready(quickAttackDelay));
			}

			//otherwise, delay after an attack
			else if(_switcher.currentWeapon == 3 && _ready)
			{
				_animator.SetTrigger("Attack");
				StartCoroutine(Ready(quickShotDelay));
			}
		}

		//Blocking Attack--------------------------------------------------------
		else if(_input._attack 
		        && _input._blocking 
		        && _ready
		        && !_convoManager.talking)
		{
			_animator.SetTrigger("BlockingAttack");
			if(blockAttackDelay > 0) StartCoroutine(Ready(blockAttackDelay));
		}

		//if bow and aim firing, start enumerator
		if(_switcher.currentWeapon == 3
		   && _input._blocking 
		   && _input._attack)
		{
			print("aiming arrow");
		}

		//foot dust
		if(_controller.isGrounded && Mathf.Abs(normalizedHorizontalSpeed) < 0.1f)
		{
			_footDust.Play();
		}

		//if dead turn off all controls, play animation
		if(_health.dead)
		{
			_animator.SetInteger("AnimState", 2);
			normalizedHorizontalSpeed = 0;
		}
	}

	//wait for the animation to end, then assign the thing
	private IEnumerator WaitForAnim(float length, AnimatorOverrideController overrider)
	{
		yield return new WaitForSeconds (length);

		_animator.runtimeAnimatorController = overrider;
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

	//takes a float speed num, changes speed based on input
	void Speed(float num)
	{
		resultSpeed = num;
	}
}
