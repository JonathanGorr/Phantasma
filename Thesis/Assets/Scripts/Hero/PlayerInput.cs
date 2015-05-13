using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {

	//components
	private LevelManager _manager;
	private ConversationManager _convoManager;
	private WeaponSwitcher _switcher;

	[HideInInspector] public bool 
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
		_healHold,
		_roll,
		_pause,
		_anyKey,
		_leftOnce,
		_rightOnce;

	[HideInInspector] public float _axis, _axisHorizontal, _axisVertical, _scrollWheel, DPadHorizontal, DPadVertical;

	void Awake()
	{
		_manager = GameObject.Find ("_LevelManager").GetComponent<LevelManager> ();
		_switcher = _manager.GetComponent<WeaponSwitcher> ();
		_convoManager = _manager.GetComponentInChildren<ConversationManager>();
	}

	public void Update()
	{
		//if paused, no input is accepted, save pause button and anykey
		if (!_manager.paused) 
		{
			//if in menu, these cannot be done
			if(!_manager.inMenu)
			{
				DPadHorizontal = Input.GetAxis ("360_HorizontalDPAD");
				DPadVertical = Input.GetAxis ("360_VerticalDPAD");
				_jump = Input.GetKeyDown (KeyCode.Space) || Input.GetButtonDown ("360_AButton") || Input.GetKey (KeyCode.DownArrow);
				_down = Input.GetMouseButtonDown (1) || Input.GetButtonDown ("360_LeftBumper") || Input.GetKeyDown (KeyCode.UpArrow);

				if(_switcher.currentWeapon != 0)
				{
					_attack = Input.GetMouseButtonDown (0) || Input.GetButtonDown ("360_RightBumper") || Input.GetKeyDown (KeyCode.J);
					_blocking = Input.GetMouseButton (1) || Input.GetButton ("360_LeftBumper") || Input.GetKey (KeyCode.UpArrow);
					_strongAttack = Input.GetKeyDown (KeyCode.E) || Input.GetAxis ("360_Triggers") > 0.6 || Input.GetKeyDown (KeyCode.G);
				}
				_backStep = Input.GetKeyDown (KeyCode.LeftControl) || Input.GetButtonDown ("360_XButton");
				_roll = Input.GetKeyDown (KeyCode.Tab) || Input.GetButtonDown ("360_BButton");
				_cycleWep = Input.GetKeyDown (KeyCode.S);
				_scrollWheel = Input.GetAxisRaw("Mouse ScrollWheel");
				_healHold = Input.GetKey(KeyCode.Q);
				_heal = Input.GetKeyDown(KeyCode.Q);

				//if the bow is out, can aim...
				if(_switcher.currentWeapon == 3)
				{
					_aiming = Input.GetKey(KeyCode.G) || Input.GetButton ("360_LeftBumper");
					if(_aiming)print("Aiming");

					//how many controller are plugged in?
					string[] joysticks = Input.GetJoystickNames ();
					
					//if there are joysticks...
					if(joysticks.Length > 0)
					{
						_axisVertical = Input.GetAxis ("360_RightStickVertical");
						_axisHorizontal = Input.GetAxis ("360_RightStickHorizontal");
					}
					//if there are no joysticks...
					else if(joysticks.Length == 0)
					{
						_axisHorizontal = Input.GetAxis ("Horizontal");
						_axisVertical = Input.GetAxis ("Vertical");
					}
				}
			}

			//if talking, stop and dont move
			if(_convoManager.talking)
			{
				_axis = 0;
				_right = false;
				_left = false;
				_roll = false;
				_leftOnce = false;
				_rightOnce = false;
			}

			//else move freely
			else
			{
				_axis = Input.GetAxis ("360_LeftStickHorizontal");
				_right = Input.GetKey (KeyCode.D) || Input.GetKey (KeyCode.RightArrow);
				_left = Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.LeftArrow);
				//
				_rightOnce = Input.GetKeyDown (KeyCode.D) || Input.GetKeyDown (KeyCode.RightArrow);
				_leftOnce = Input.GetKeyDown (KeyCode.A) || Input.GetKeyDown (KeyCode.LeftArrow);
			}
		}

		//if not in menu, can pause
		if(!_manager.inMenu)
		{
			_pause = Input.GetKeyDown (KeyCode.Escape) || Input.GetButtonDown ("360_StartButton") || Input.GetKeyDown (KeyCode.F);
		}

		_anyKey = Input.anyKeyDown;
	}
}