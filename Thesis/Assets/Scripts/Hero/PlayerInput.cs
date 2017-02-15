using UnityEngine;
using System.Collections;

public class PlayerInput : WaitForPlayer {

	//components
	private ConversationManager _convoManager;
	private WeaponController _switcher;
	private Health _playerHealth;

	[HideInInspector] public bool 
		_controller,
		_cycleWep,
		_blocking,
		_anyKeyDown;

	[HideInInspector] public float 
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
	bool _l2;
	public delegate void LeftTrigger();
	public event LeftTrigger onLeftTrigger;

	void LTrigger()
	{
		if(!_l2 && Input.GetAxis("360_Triggers") < 0f)
			if(onLeftTrigger != null) onLeftTrigger();
			_l2 = true; 
		while(Input.GetAxis("360_Triggers") < 0f && _l2) return;
		_l2 = false;
	}

	//right trigger
	bool _rt;
	public delegate void RightTrigger();
	public event RightTrigger onRightTrigger;

	void RTrigger()
	{
		if(!_rt && Input.GetAxis("360_Triggers") > 0f)
		if(onRightTrigger != null)
			 onRightTrigger(); 
			 	_rt = true;
		while(Input.GetAxis("360_Triggers") > 0f && _rt) return;
		_rt = false;
	}

	//DPAD
	bool _pad;
	public enum PADDirection { left, right, up, down }
	public delegate void DPAD(PADDirection d);
	public event DPAD onPad;
	void Dpad()
	{
		//left
		if(Input.GetAxis ("360_HorizontalDPAD") < 0 && !_pad){
			if(onPad != null) onPad(PADDirection.left);
			_pad = true;
		}
		//right
		else if(Input.GetAxis ("360_HorizontalDPAD") > 0 && !_pad){
			if(onPad != null) onPad(PADDirection.right);
			_pad = true;
		}
		//up
		else if(Input.GetAxis ("360_VerticalDPAD") > 0 && !_pad){
			if(onPad != null) onPad(PADDirection.up);
			_pad = true; 
		}
		//down
		else if(Input.GetAxis ("360_VerticalDPAD") < 0 && !_pad){
			if(onPad != null) onPad(PADDirection.down);
			_pad = true;
		}
		while((Mathf.Abs(Input.GetAxis("360_VerticalDPAD")) > 0f || Mathf.Abs(Input.GetAxis("360_HorizontalDPAD")) > 0f) && _pad) { return; }
		_pad = false;
	}

	//X (square) Button
	bool x;
	public delegate void OnX();
	public event OnX onX;
	void X()
	{
		if(Input.GetButtonDown ("360_XButton") && !x)
		{
			if(onX != null) onX();
			x = true;
		}
		while(Input.GetButtonDown ("360_XButton") && x) { return; }
		x = false;
	}

	//A (X) button
	bool a;
	public delegate void OnA();
	public event OnA onA;
	void A()
	{
		if(Input.GetButtonDown ("360_AButton") && !a)
		{
			if(onA != null) onA();
			a = true;
		}
		while(Input.GetButtonDown ("360_AButton") && a) { return; }
		a = false;
	}

	//B (Circle) button
	bool b;
	public delegate void OnB();
	public event OnB onB;
	void B()
	{
		if(Input.GetButtonDown("360_BButton") && !b)
		{
			if(onB != null) onB();
			b = true;
		}
		while(Input.GetButtonDown("360_BButton") && b) { return; }
		b = false;
	}

	//B (Circle) button
	bool y;
	public delegate void OnY();
	public event OnY onY;
	void Y()
	{
		if(Input.GetButtonDown("360_YButton") && !y)
		{
			if(onY != null) onY();
			y = true;
		}
		while(Input.GetButtonDown("360_YButton") && y) { return; }
		y = false;
	}

	//Start (>) button
	bool start;
	public delegate void OnStart();
	public event OnStart onStart;
	void Start()
	{
		if(Input.GetButtonDown ("360_StartButton") && !start)
		{
			if(onStart != null) onStart();
			start = true;
		}
		while(Input.GetButtonDown ("360_StartButton") && start) { return; }
		start = false;
	}

	//Select ([ ]) button
	bool select;
	public delegate void OnSelect();
	public event OnSelect onSelect;
	void Select()
	{
		if(Input.GetButtonDown ("360_BackButton") && !select)
		{
			if(onSelect != null) onSelect();
			select = true;
		}
		while(Input.GetButtonDown ("360_BackButton") && select) { return; }
		select = false;
	}

	//L1 button
	public bool L1Down
	{
		get { while(Input.GetButton("360_LeftBumper")) return true;
		return false; }
	}
	bool _l1;
	public delegate void OnL1();
	public event OnL1 onL1;
	void L1()
	{
		if(Input.GetButtonDown ("360_LeftBumper") && !_l1)
		{
			if(onL1 != null) onL1();
			_l1 = true;
		}
		while(Input.GetButtonDown ("360_LeftBumper") && _l1) { return; }
		_l1 = false;
	}

	//R1 button
	public bool R1Down
	{
		get { while(Input.GetButton("360_RightBumper")) return true;
		return false; }
	}
	bool _r1;
	public delegate void OnR1();
	public event OnR1 onR1;
	void R1()
	{
		if(Input.GetButtonDown ("360_RightBumper") && !_r1)
		{
			if(onR1 != null) onR1();
			_r1 = true;
		}
		while(Input.GetButtonDown ("360_RightBumper") && _r1) { return; }
		_r1 = false;
	}

	public Vector2 RAnalog
	{
		get { return new Vector2(Input.GetAxis ("360_RightStickHorizontal"), Input.GetAxis ("360_RightStickVertical")); }
	}
	public Vector2 LAnalog
	{
		get { return new Vector2(Input.GetAxis ("360_LeftStickHorizontal"), Input.GetAxis ("360_LeftStickVertical")); }
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
		Start();
		Select();

		//bool true if theres a controller
		_controller = (Input.GetJoystickNames ().Length == 1) ? true : false;
		_anyKeyDown = Input.anyKeyDown;
	}
}