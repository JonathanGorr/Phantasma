using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

//triggers
//analog buttons

/// <summary>
/// Player input:
/// Used to separate controls for different input peripherals
/// buttons are consistently named, but prefaced by the enum name so that they may listen to different input strings
/// </summary>

/*
//Android keycodes:
//Buttons
KEYCODE_BUTTON_A = keycode 96(`) joystick button 0
KEYCODE_BUTTON_B = Keycode 97 joystick button 1
KEYCODE_BUTTON_X = Keycode 99 joystick button 2
KEYCODE_BUTTON_Y = Keycode 100 joystick button 3

KEYCODE_BUTTON_L1 = Keycode 102 joystick button 4
KEYCODE_BUTTON_R1 = Keycode 103 joystick button 5

KEYCODE_BUTTON_L3 = Keycode 108 joystick button 8
KEYCODE_BUTTON_R3 = Keycode 109 joystick button 9

KEYCODE_BUTTON_OPTIONS = Keycode 105 escape
KEYCODE_BUTTON_SHARE = Keycode 104 joystick button 10

Axes
LANALOG_HORIZONTAL = AXIS_X 
LANALOG_VERTICAL = AXIS_Y

LTRIGGER = AXIS_RX or joystick button 6
RTRIGGER = AXIS_RY or joystick button 7

DPAD_HORIZONTAL = AXIS_HAT_X
DPAD_VERTICAL = AXIS_HAT_Y

RANALOG_HORIZONTAL = AXIS_Z
RANALOG_VERTICAL = AXIS_RZ
*/

public class PlayerInput : WaitForPlayer {

	//this is a map for keycodes associated with different devices( most importantly PC vs Android )
	class ControllerMap
	{
		public KeyCode A{ get; }
		public KeyCode B{ get; }
		public KeyCode X{ get; }
		public KeyCode Y{ get; }

		public KeyCode L1{ get; }
		public KeyCode R1{ get; }

		public KeyCode L3{ get; }
		public KeyCode R3{ get; }

		public KeyCode Start{ get; }
		public KeyCode Select{ get; }

		public KeyCode Lt{ get; }
		public KeyCode Rt{ get; }

		public ControllerMap(KeyCode a, KeyCode b, KeyCode x, KeyCode y, KeyCode l1,
		 KeyCode r1, KeyCode l3, KeyCode r3, KeyCode start, KeyCode select, KeyCode lt, KeyCode rt)
		{
			A = a;
			B = b;
			X = x;
			Y = y;

			L1 = l1;
			R1 = r1;

			L3 = l3;
			R3 = r3;

			Start = start;
			Select = select;

			Lt = lt;
			Rt = rt;
		}
	}

	ControllerMap pcMap;
	ControllerMap androidMap;

	public bool debug = false;

	public static PlayerInput Instance = null;

	public enum InputState { Controller, MouseKeyboard }
	public InputState inputState = InputState.MouseKeyboard;

	//substate for specific controller
	public enum ControllerState {XBOX,DS4,Android}
	public ControllerState controllerState = ControllerState.XBOX;

	public delegate void InputSwitched( string inP );
	public static event InputSwitched onSwitch;

	void OnGUI()
	{
		switch( inputState )
         {
             case InputState.MouseKeyboard: //if mouse and detect controller...switch
                 if(IsControllerInput())
                 {
					inputState = InputState.Controller;
                 	if(Application.isMobilePlatform) //there's only really 1 keymapping for controllers interpreted by android
                 	{
						controllerState = ControllerState.Android;
                 	}
                 	else
                 	{
						switch(Input.GetJoystickNames()[0].Length) // 1st controller read
	                 	{
							case 19:// "Wireless Controller"
	                 		controllerState = ControllerState.DS4;
	                 		break;
							case 33:// "Controller (XBOX XBOX For Windows)"
							controllerState = ControllerState.XBOX;
	                 		break;
	                 		default://if unrecognized, assume it is a xbox controller...
	                 		controllerState = ControllerState.XBOX;
	                 		break;
	                 	}
                 	}
                 	//set event system buttons
                 	DebugWindow.Instance.InputState(inputState);
                 	if(onSwitch != null) onSwitch(controllerState.ToString());
	                if(debug) print("Input - " + controllerState.ToString() + " controller is being used");
                 }
                 break;

			case InputState.Controller:  //if controller and detect mouse or different controller...switch
                 if (IsMouseKeyboard())
                 {
					inputState = InputState.MouseKeyboard;
					if(onSwitch != null) onSwitch(inputState.ToString());
					if(debug) print("Input - Mouse & Keyboard being used");
                 }
                 break;
         }
	}

	//checks keyboard mouse input to switch to that
	private bool IsMouseKeyboard()
    {
         // mouse & keyboard buttons
         if (Event.current.isKey ||
             Event.current.isMouse)
         {
             return true;
         }
         // mouse movement
         if( Input.GetAxis("Mouse X") != 0.0f ||
             Input.GetAxis("Mouse Y") != 0.0f )
         {
             return true;
         }
         return false;
     }

	//returns true if a controller input is detected
	private bool IsControllerInput()
    {
         // joystick buttons
         if(Input.GetKey(KeyCode.Joystick1Button0)  ||
            Input.GetKey(KeyCode.Joystick1Button1)  ||
            Input.GetKey(KeyCode.Joystick1Button2)  ||
            Input.GetKey(KeyCode.Joystick1Button3)  ||
            Input.GetKey(KeyCode.Joystick1Button4)  ||
            Input.GetKey(KeyCode.Joystick1Button5)  ||
            Input.GetKey(KeyCode.Joystick1Button6)  ||
            Input.GetKey(KeyCode.Joystick1Button7)  ||
            Input.GetKey(KeyCode.Joystick1Button8)  ||
            Input.GetKey(KeyCode.Joystick1Button9)  ||
            Input.GetKey(KeyCode.Joystick1Button10) ||
            Input.GetKey(KeyCode.Joystick1Button11) ||
            Input.GetKey(KeyCode.Joystick1Button12) ||
            Input.GetKey(KeyCode.Joystick1Button13) ||
            Input.GetKey(KeyCode.Joystick1Button14) ||
            Input.GetKey(KeyCode.Joystick1Button15) ||
            Input.GetKey(KeyCode.Joystick1Button16) ||
            Input.GetKey(KeyCode.Joystick1Button17) ||
            Input.GetKey(KeyCode.Joystick1Button18) ||
            Input.GetKey(KeyCode.Joystick1Button19) )
         {
             return true;
         }

         // joystick axis
		if(
			Input.GetAxis(controllerState.ToString() + "_LeftStickHorizontal") != 0.0f ||
			Input.GetAxis(controllerState.ToString() + "_RightStickHorizontal") != 0.0f ||
			Input.GetAxis(controllerState.ToString() + "_LeftTrigger") != 0.0f ||
			Input.GetAxis(controllerState.ToString() + "_RightTrigger") != 0.0f ||
			Input.GetAxis(controllerState.ToString() + "_HorizontalDPAD") != 0.0f ||
			Input.GetAxis(controllerState.ToString() + "_VerticalDPAD") != 0.0f

			)
         {
             return true;
         }
         
         return false;
    }

	void Awake()
	{
		if(Instance == null) Instance = this;
		//debug = false;
		if(debug) Debug();
	}

	void Debug()
	{
		//print list of controllers
		string[] names = Input.GetJoystickNames();
		for(int i=0;i<names.Length;i++)
		{
			print(names[i]);
		}
	}

	public override IEnumerator Initialize(UnityEngine.SceneManagement.Scene scene)
	{
		while(Player.Instance == null) yield return null;
	}

	string LogLeftAnalog()
	{
		string la = "";
		if(Mathf.Abs(LAnalog.x) > 0.1f || Mathf.Abs(LAnalog.y) > 0.1f)
		{
			//left stick corners
			if(LAnalog.x > 0
			&& LAnalog.y > 0)
			{
				la = "NE";
			}
			else if(LAnalog.x > 0
			&& LAnalog.y < 0)
			{
				la = "SE";
			}
			else if(LAnalog.x < 0 
			&& LAnalog.y > 0)
			{
				la = "NW";
			}
			else if(LAnalog.x < 0
			&& LAnalog.y < 0)
			{
				la = "SW";
			}
		}
		return la;
	}

	string LogRightAnalog()
	{
		string ra = "";
		if(Mathf.Abs(RAnalog.x) > 0.1f || Mathf.Abs(RAnalog.y) > 0.1f)
		{
			//right stick
			if(RAnalog.x > 0 
			&& RAnalog.y > 0)
			{
				ra = "NE";
			}
			else if(RAnalog.x > 0
			&& RAnalog.y < 0)
			{
				ra = "SE";
			}
			else if(RAnalog.x < 0 
			&& RAnalog.y > 0)
			{
				ra = "NW";
			}
			else if(RAnalog.x < 0
			&& RAnalog.y < 0)
			{
				ra = "SW";
			}
		}
		return ra;
	}

	void Update()
	{
		LTrigger();
		RTrigger();
		Dpad();
		X();
		A();
		B();
		Y();
		L1();
		R1();
		StartButton();
		Select();
	
		if(DebugWindow.Instance.isActiveAndEnabled)
		{
			DebugWindow.Instance.Axes(LogLeftAnalog(), LogRightAnalog());
		}
	}

	//left trigger
	public static bool _lt;
	public delegate void LeftTrigger();
	public static event LeftTrigger onLeftTrigger;

	void LTrigger()
	{
		if(Input.GetAxis(controllerState.ToString() + "_LeftTrigger") > 0f && !_lt)
		{
			if(onLeftTrigger != null) onLeftTrigger();
			if(DebugWindow.Instance.isActiveAndEnabled) DebugWindow.Instance.Log(controllerState.ToString() + "_LeftTrigger");
			_lt = true;
		}
		while(Input.GetAxis(controllerState.ToString() + "_LeftTrigger") > 0f && _lt)
		{
			return;
		}
		_lt = false;
	}

	//right trigger
	public static bool _rt;
	public delegate void RightTrigger();
	public static event RightTrigger onRightTrigger;

	void RTrigger()
	{
		if(Input.GetAxis(controllerState.ToString() + "_RightTrigger") > 0f && !_rt)
		{
			if(DebugWindow.Instance.isActiveAndEnabled) DebugWindow.Instance.Log(controllerState.ToString() + "_RightTrigger");
			if(onRightTrigger != null) onRightTrigger();
			_rt = true;
		}
		while(Input.GetAxis(controllerState.ToString() + "_RightTrigger") > 0f && _rt)
		{
			return;
		}
		_rt = false;
	}

	//DPAD
	[HideInInspector] public static bool _pad;
	public enum PADDirection { left, right, up, down }
	public delegate void DPAD(PADDirection d);
	public static event DPAD onPad;
	void Dpad()
	{
		//left
		if(Input.GetAxis (controllerState.ToString() + "_HorizontalDPAD") < 0 && !_pad){
			if(DebugWindow.Instance.isActiveAndEnabled) DebugWindow.Instance.Log(controllerState.ToString() + "_DPAD Left");
			if(onPad != null) onPad(PADDirection.left);
			_pad = true;
		}
		//right
		else if(Input.GetAxis (controllerState.ToString() + "_HorizontalDPAD") > 0 && !_pad){
			if(DebugWindow.Instance.isActiveAndEnabled) DebugWindow.Instance.Log(controllerState.ToString() + "_DPAD Right");
			if(onPad != null) onPad(PADDirection.right);
			_pad = true;
		}
		//up
		else if(Input.GetAxis (controllerState.ToString() + "_VerticalDPAD") > 0 && !_pad){
			if(DebugWindow.Instance.isActiveAndEnabled) DebugWindow.Instance.Log(controllerState.ToString() + "_DPAD Up");
			if(onPad != null) onPad(PADDirection.up);
			_pad = true; 
		}
		//down
		else if(Input.GetAxis (controllerState.ToString() + "_VerticalDPAD") < 0 && !_pad){
			if(DebugWindow.Instance.isActiveAndEnabled) DebugWindow.Instance.Log(controllerState.ToString() + "_DPAD Down");
			if(onPad != null) onPad(PADDirection.down);
			_pad = true;
		}
		while((Mathf.Abs(Input.GetAxis(controllerState.ToString() + "_VerticalDPAD")) > 0f || Mathf.Abs(Input.GetAxis(controllerState.ToString() + "_HorizontalDPAD")) > 0f) && _pad) { return; }
		_pad = false;
	}

	//X (square) Button
	public static bool x;
	public delegate void OnX();
	public static event OnX onX;
	void X()
	{
		if(Input.GetButtonDown (controllerState.ToString() + "_XButton") && !x)
		{
			if(DebugWindow.Instance.isActiveAndEnabled) DebugWindow.Instance.Log(controllerState.ToString() + "_XButton");
			if(onX != null) onX();
			x = true;
		}
		while(Input.GetButton(controllerState.ToString() + "_XButton") && x) return; 
		x = false;
	}

	//A button
	public bool ADown
	{
		get { while(Input.GetButton(controllerState.ToString() + "_AButton")) return true;
		return false; }
	}

	//A (X) button
	public static bool a;
	public delegate void OnA();
	public static event OnA onA;
	void A()
	{
		if(Input.GetButtonDown (controllerState.ToString() + "_AButton") && !a)
		{
			if(DebugWindow.Instance.isActiveAndEnabled) DebugWindow.Instance.Log(controllerState.ToString() + "_AButton");
			if(onA != null) onA();
			a = true;
		}
		while(Input.GetButton(controllerState.ToString() + "_AButton") && a) { return; }
		a = false;
	}

	//B (Circle) button
	public static bool b;
	public delegate void OnB();
	public static event OnB onB;
	void B()
	{
		if(Input.GetButtonDown(controllerState.ToString() + "_BButton") && !b)
		{
			if(DebugWindow.Instance.isActiveAndEnabled) DebugWindow.Instance.Log(controllerState.ToString() + "_BButton");
			if(onB != null) onB();
			b = true;
		}
		while(Input.GetButton(controllerState.ToString() + "_BButton") && b) { return; }
		b = false;
	}

	//B (Circle) button
	public static bool y;
	public delegate void OnY();
	public static event OnY onY;
	void Y()
	{
		if(Input.GetButtonDown(controllerState.ToString() + "_YButton") && !y)
		{
			if(DebugWindow.Instance.isActiveAndEnabled) DebugWindow.Instance.Log(controllerState.ToString() + "_YButton");
			if(onY != null) onY();
			y = true;
		}
		while(Input.GetButton(controllerState.ToString() + "_YButton") && y) { return; }
		y = false;
	}

	//Start (>) button
	public static bool start;
	public delegate void OnStart();
	public static event OnStart onStart;
	void StartButton()
	{
		if(Input.GetButtonDown (controllerState.ToString() + "_StartButton") && !start)
		{
			if(DebugWindow.Instance.isActiveAndEnabled) DebugWindow.Instance.Log(controllerState.ToString() + "_StartButton");
			if(onStart != null) onStart();
			start = true;
		}
		while(Input.GetButton(controllerState.ToString() + "_StartButton") && start) { return; }
		start = false;
	}

	//Select ([ ]) button
	public static bool select;
	public delegate void OnSelect();
	public static event OnSelect onSelect;
	void Select()
	{
		if(Input.GetButtonDown (controllerState.ToString() + "_SelectButton") && !select)
		{
			if(DebugWindow.Instance.isActiveAndEnabled) DebugWindow.Instance.Log(controllerState.ToString() + "_SelectButton");
			if(onSelect != null) onSelect();
			select = true;
		}
		while(Input.GetButton(controllerState.ToString() + "_SelectButton") && select) { return; }
		select = false;
	}


	//L1 button
	public bool L1Down
	{
		get { while(Input.GetButton(controllerState.ToString() + "_LeftBumper")) return true;
		return false; }
	}

	public static bool _l1;
	public delegate void OnL1();
	public static event OnL1 onL1;
	void L1()
	{
		if(Input.GetButtonDown (controllerState.ToString() + "_LeftBumper") && !_l1)
		{
			if(DebugWindow.Instance.isActiveAndEnabled) DebugWindow.Instance.Log(controllerState.ToString() + "_LeftBumper");
			if(onL1 != null) onL1();
			_l1 = true;
		}
		while(Input.GetButton(controllerState.ToString() + "_LeftBumper") && _l1) { return; }
		_l1 = false;
	}


	//L1 button
	public bool R1Down
	{
		get { while(Input.GetButton(controllerState.ToString() + "_RightBumper")) return true;
		return false; }
	}

	public static bool _r1;
	public delegate void OnR1();
	public static event OnR1 onR1;
	void R1()
	{
		if(Input.GetButtonDown (controllerState.ToString() + "_RightBumper") && !_r1)
		{
			if(DebugWindow.Instance.isActiveAndEnabled) DebugWindow.Instance.Log(controllerState.ToString() + "_RightBumper");
			if(onR1 != null) onR1();
			_r1 = true;
		}
		while(Input.GetButton(controllerState.ToString() + "_RightBumper") && _r1) { return; }
		_r1 = false;
	}

	public Vector2 RAnalog
	{
		get { 
		return new Vector2(
			Input.GetAxis (controllerState.ToString() + "_RightStickHorizontal"),
			Input.GetAxis (controllerState.ToString() + "_RightStickVertical")); 
		}
	}
	public Vector2 LAnalog
	{
		get {
		return new Vector2(
			Input.GetAxis (controllerState.ToString() + "_LeftStickHorizontal"),
			Input.GetAxis (controllerState.ToString() + "_LeftStickVertical")); 
		 }
	}
}