using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {

	//components
	private LevelManager _manager;
	private ConversationManager _convoManager;
	private WeaponSwitcher _switcher;
	private Health _playerHealth;

	[HideInInspector] public bool 
		_controller,
		_left,
		_right,
		_up,
		_down,
		_jump,
		_attack,
		_strongAttack,
		_aiming,
		_cycleWep,
		_blocking,
		_backStep,
		_heal,
		_roll,
		_pause,
		_anyKeyDown,
		_leftOnce,
		_rightOnce,
		_submit;

	[HideInInspector] public float 
		_axis,
		_axisHorizontal,
		_axisVertical,
		_scrollWheel,
		_DPadHorizontal,
		_DPadVertical;

	void Awake()
	{
		_manager = GameObject.Find ("_LevelManager").GetComponent<LevelManager> ();
		_switcher = _manager.GetComponent<WeaponSwitcher> ();
		_convoManager = _manager.GetComponentInChildren<ConversationManager>();
		_playerHealth = GameObject.Find ("_Player").GetComponent<Health> ();
	}

	void Update()
	{
		//bool true if theres a controller
		_controller = (Input.GetJoystickNames ().Length == 1) ? true : false;

		//if player is not dead...
		if(!_playerHealth.dead)
		{
			if(_controller)
			{
				_pause = Input.GetButtonDown ("360_StartButton");
				_submit = Input.GetButtonDown("360_AButton");
			}
			else
			{
				_pause = Input.GetKeyDown (KeyCode.Escape);
				_submit = Input.GetButtonDown("Submit");
			}

			//if paused, only menu input is accepted
			if (!_manager.paused)
			{
				//if not talking...
				if(!_convoManager.talking)
				{
					//if controller is present
					if(_controller)
					{
						_axis = Input.GetAxis ("360_LeftStickHorizontal");

						_right = false;
						_left = false;
						_rightOnce = false;
						_leftOnce = false;
						
						_DPadHorizontal = Input.GetAxis ("360_HorizontalDPAD");
						_DPadVertical = Input.GetAxis ("360_VerticalDPAD");
						_jump = Input.GetButtonDown ("360_AButton");
						_down = Input.GetButtonDown ("360_LeftBumper");
						_backStep = Input.GetButtonDown ("360_XButton");
						_roll = Input.GetButtonDown ("360_BButton");
						_cycleWep = false;
						_scrollWheel = 0;
						_heal = Input.GetButtonDown ("360_YButton");
					}

					//no controller
					else
					{
						_axis = 0;
						_right = Input.GetKey (KeyCode.D);
						_left = Input.GetKey (KeyCode.A);
						_rightOnce = false;
						_leftOnce = false;
						
						_DPadHorizontal = 0;
						_DPadVertical = 0;

						_jump = Input.GetKeyDown (KeyCode.Space);
						_down = false;
						_backStep = Input.GetKeyDown (KeyCode.LeftControl);
						_roll = Input.GetKeyDown (KeyCode.LeftShift);
						_cycleWep = Input.GetKeyDown (KeyCode.E);
						_scrollWheel = Input.GetAxisRaw("Mouse ScrollWheel");
						_heal = Input.GetKeyDown(KeyCode.Q);
					}
					
					//if the bow is out, can aim...
					if(_switcher.currentWeapon == 3)
					{
						//controller
						if(_controller)
						{
							_attack = Input.GetButtonDown ("360_RightBumper");
							_aiming = Input.GetButton("360_LeftBumper");
						}

						//no controller
						else
						{
							_attack = Input.GetMouseButtonDown(0);
							_aiming = Input.GetMouseButton(1);
						}

						//if aiming
						if(_aiming)
						{
							//print("Aiming");
						
							//controller
							if(_controller)
							{
								_axisVertical = Input.GetAxis ("360_RightStickVertical");
								_axisHorizontal = Input.GetAxis ("360_RightStickHorizontal");
							}

							//No controller
							else
							{
								_axisHorizontal = Input.mousePosition.x;
								_axisVertical = Input.mousePosition.y;
							}
						}
					}

					//if you are not bare-Fisted
					else if(_switcher.currentWeapon != 0)
					{
						//controller
						if(_controller)
						{
							_attack = Input.GetButtonDown ("360_RightBumper");
							_blocking = Input.GetButton ("360_LeftBumper");
							_strongAttack = Input.GetAxis ("360_Triggers") > 0.9;
						}

						//no controller
						else
						{
							_attack = Input.GetMouseButtonDown (0);

							//if the sword is out, hold right mouse to block
							if(_switcher.currentWeapon == 1)
								_blocking = Input.GetMouseButton (1);

							//right mouse strong attacks for every other weapon
							else
							{
								_blocking = false;
								_strongAttack = Input.GetMouseButtonDown (1);
							}
						}
					}

					//if in menu...
					if(_manager.inMenu)
					{
						_jump = false;
					}
				}

				//else if talking set these to stop
				else
				{
					_axis = 0;
					_right = false;
					_left = false;
					_roll = false;
					_leftOnce = false;
					_rightOnce = false;
				}
			}
		}
		_anyKeyDown = Input.anyKeyDown;
	}
}