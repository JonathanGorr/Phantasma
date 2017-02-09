using UnityEngine;
using System.Collections;

public class PlayerInput : WaitForPlayer {

	//components
	private ConversationManager _convoManager;
	private WeaponController _switcher;
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
		_anyKeyDown,
		_leftOnce,
		_rightOnce,
		_select,
		_pause;

	[HideInInspector] public float 
		_axis,
		_axisHorizontal,
		_axisVertical,
		_scrollWheel,
		_DPadHorizontal,
		_DPadVertical;

	public override IEnumerator Initialize(UnityEngine.SceneManagement.Scene scene)
	{
		while(_manager.Player == null) yield return null;

		_switcher = _manager.Player.GetComponent<WeaponController>();
		_convoManager = GetComponent<ConversationManager>();
		_playerHealth = _manager.Player.GetComponent<Health>();
	}

	//left trigger
	bool _lt;
	public delegate void LeftTrigger();
	public event LeftTrigger onLeftTrigger;

	void LTrigger()
	{
		if(!_lt && Input.GetAxis("360_Triggers") < 0f){ onLeftTrigger(); _lt = true; }
		while(Input.GetAxis("360_Triggers") < 0f && _lt) return;
		_lt = false;
	}

	//right trigger
	bool _rt;
	public delegate void RightTrigger();
	public event RightTrigger onRightTrigger;

	void RTrigger()
	{
		if(!_rt && Input.GetAxis("360_Triggers") > 0f){ onLeftTrigger(); _rt = true; }
		while(Input.GetAxis("360_Triggers") > 0f && _rt) return;
		_rt = false;
	}

	void Update()
	{
		if(!_switcher) return;
		if(!_playerHealth) return;

		LTrigger();
		RTrigger();

		//bool true if theres a controller
		_controller = (Input.GetJoystickNames ().Length == 1) ? true : false;

		//if player is not dead...
		if(!_playerHealth.dead)
		{
			if(_controller)
			{
				_pause = Input.GetButtonDown ("360_StartButton");
				_select = Input.GetButtonDown("360_BackButton");
			}
			else
			{
				_pause = Input.GetKeyDown (KeyCode.Escape);
				_select = Input.GetButtonDown("Submit");
			}

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
				
				//if the bow is out, can aim...
				if(_switcher.IsWeapon(Weapons.Bow))
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
				}

				//if you are not bare-Fisted
				else if(!_switcher.IsWeapon(Weapons.Empty))
				{
					//controller
					if(_controller)
					{
						_attack = Input.GetButtonDown ("360_RightBumper");
						_blocking = Input.GetButton ("360_LeftBumper");
						_strongAttack = Input.GetAxis ("360_Triggers") > 0.99f;
					}

					//no controller
					else
					{
						_attack = Input.GetMouseButtonDown (0);

						//if the sword is out, hold right mouse to block
						if(_switcher.IsWeapon(Weapons.SwordShield))
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
				if(currentSceneName != "Start")
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
		_anyKeyDown = Input.anyKeyDown;

	}
}