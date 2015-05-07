using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {

	[HideInInspector] public bool 
		_left, 
		_right, 
		_up, 
		_down, 
		_jump, 
		_attack, 
		_strongAttack, 
		_cycleWep,
		_blocking,
		_backStep,
		_heal, 
		_roll,
		_pause,
		_anyKey;

	[HideInInspector] public float _axis;

	public void Update()
	{
		//getkeydown = once; getkey = continuous
		_jump = Input.GetKeyDown( KeyCode.Space) || Input.GetButtonDown("360_AButton") || Input.GetKey(KeyCode.DownArrow);
		_right = Input.GetKey (KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
		_left = Input.GetKey (KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
		_attack = Input.GetMouseButtonDown (0) || Input.GetButtonDown("360_RightBumper") || Input.GetKeyDown(KeyCode.J);
		_blocking = Input.GetMouseButton (1) || Input.GetButton("360_LeftBumper") || Input.GetKey(KeyCode.UpArrow);
		_strongAttack = Input.GetKeyDown(KeyCode.E) || Input.GetAxis("360_Triggers") > 0.6 || Input.GetKeyDown(KeyCode.G);
		_backStep = Input.GetKeyDown (KeyCode.LeftControl) || Input.GetButtonDown("360_XButton");
		_roll = Input.GetKeyDown (KeyCode.Tab) || Input.GetButtonDown ("360_BButton");
		_axis = Input.GetAxis ("360_LeftStickHorizontal");
		_cycleWep = Input.GetKeyDown(KeyCode.S);
		_pause = Input.GetKeyDown (KeyCode.Escape) || Input.GetButtonDown ("360_StartButton") || Input.GetKeyDown (KeyCode.F);
		_anyKey = Input.anyKeyDown;
	}
}