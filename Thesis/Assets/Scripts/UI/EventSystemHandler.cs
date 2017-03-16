using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemHandler : MonoBehaviour {

	public static EventSystemHandler Instance = null;

	public StandaloneInputModule KeyboardMouse;
	public StandaloneInputModule XBOXController;
	public StandaloneInputModule DS4Controller;
	public StandaloneInputModule AndroidController;

	private StandaloneInputModule current;
	public StandaloneInputModule CurrenInputModule
	{
		get { return current; }
		set { current = value; }
	}

	void Awake()
	{
		if(Instance == null) Instance = this;
		PlayerInput.onSwitch += SetInput;
	}

	public void SetInput(string inP)
	{
		switch(inP)
		{
			case "XBOX":
				KeyboardMouse.enabled = false;
				XBOXController.enabled = true;
				DS4Controller.enabled = false;
				AndroidController.enabled = false;
				CurrenInputModule = XBOXController;
			break;
			case "DS4":
				KeyboardMouse.enabled = false;
				XBOXController.enabled = false;
				DS4Controller.enabled = true;
				AndroidController.enabled = false;
				CurrenInputModule = DS4Controller;
			break;
			case "KeyboardMouse":
				KeyboardMouse.enabled = true;
				XBOXController.enabled = false;
				DS4Controller.enabled = false;
				AndroidController.enabled = false;
				CurrenInputModule = KeyboardMouse;
			break;
			case "Android":
				KeyboardMouse.enabled = false;
				XBOXController.enabled = false;
				DS4Controller.enabled = false;
				AndroidController.enabled = true;
				CurrenInputModule = AndroidController;
			break;
		}
	}
}
