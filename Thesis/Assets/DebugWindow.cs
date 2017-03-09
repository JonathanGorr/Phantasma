using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class DebugWindow : MonoBehaviour {

	public static DebugWindow Instance = null;

	bool active = false;
	public CanvasGroup debugWindowCG;

	public Text sceneText;
	public Text inputStateText;
	public Text inputText;
	public Text axesText;
	public Text joysticksText;
	public Text frameRateText;

	void Awake()
	{
		if(Instance == null) Instance = this;
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode m)
	{
		sceneText.text = "Scene: " + scene.name;
	}

	void Update () 
	{
	#if UNITY_EDITOR
		if(Input.GetKeyDown(KeyCode.Space))
		{
			if(!active) SetWindowVisible(true);
			else SetWindowVisible(false);
		}
	#endif
		FrameLog();
		InputLog();
	}

	public void SetWindowVisible(bool visible)
	{
		active = visible;
		if(active) { enabled = true; debugWindowCG.TurnOn(); }
		else { enabled = false; debugWindowCG.TurnOff(); }
	}

	public void Log(string inP)
	{
		inputText.text = "Input: " + inP;
	}

	public void InputState(PlayerInput.InputState state)
	{
		inputStateText.text = "Input State: " + state.ToString();
	}

	public void Axes(string l, string r)
	{
		axesText.text = "LeftStick: " + l + " & " + "RightStick: " + r;
	}

	void InputLog()
	{
		joysticksText.text = "Joysticks: " + Input.GetJoystickNames().Length + " " + PlayerInput.Instance.controllerState.ToString();

		if(!Application.isMobilePlatform)
		{
			foreach(KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
	     	{
		         if (Input.GetKeyDown(kcode)) Log(kcode.ToString());
	     	}
     	}
	}

	//FRAMERATE
	public  float updateInterval = 0.5F;
	private float accum   = 0; // FPS accumulated over the interval
	private int   frames  = 0; // Frames drawn over the interval
	private float timeleft; // Left time for current interval
	 
	void Start()
	{
	    timeleft = updateInterval;
	}
	 
	void FrameLog()
	{
	    timeleft -= Time.deltaTime;
	    accum += Time.timeScale/Time.deltaTime;
	    ++frames;
	 
	    // Interval ended - update GUI text and start new interval
	    if( timeleft <= 0.0 )
	    {
	        // display two fractional digits (f2 format)
			float fps = Mathf.FloorToInt(accum/frames);
			frameRateText.text = "FrameRate: " + fps.ToString();
		 
			if(fps < 30)
				frameRateText.color = Color.yellow;
			else if(fps < 10)
				frameRateText.color = Color.red;
			else
				frameRateText.color = Color.green;

		        timeleft = updateInterval;
		        accum = 0.0F;
		        frames = 0;
	    }
    }
}
