using UnityEngine;
using System.Collections;

public class ControllerUIEvent : MonoBehaviour {

	[SerializeField, HideInInspector]
	public GameObject _pauseScreen, _deathScreen, _dialogScreen;
	private LevelManager _pause;

	// Use this for initialization
	void Awake () {
		_deathScreen = GameObject.Find ("DeathMessage");
		_dialogScreen = GameObject.Find ("Dialog");
		_pauseScreen = GameObject.Find ("PauseMenu");
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		/*
		if(_pauseScreen.activeSelf == true)
		{
			print ("pause is the active window");
		}
		else if(_deathScreen.activeSelf == true)
		{
			print ("death is the active window");
		}
		*/
	}
}
