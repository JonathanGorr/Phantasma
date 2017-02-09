using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour {

	LevelManager _manager;
	EventSystem _eventSystem;
	public GameObject newGameButton;
	public GameObject continueButton;
	public GameObject quitButton;

	EventSystem EventSystem
	{
		get
		{
			if(_eventSystem == null)
			{
				_eventSystem = _manager.es;
			}
			return _eventSystem;
		}
	}

	void Start()
	{
		_manager = GameObject.Find("_LevelManager").GetComponent<LevelManager>();
		//set the current button to new game or continue based on memory
		SetMenu();
	}

	//called whenever unity application window loses focus
	void OnApplicationFocus(bool hasFocus)
	{
		if(hasFocus)
		{
			SetMenu();
		}
	}

	void SetMenu()
	{
		EventSystem.firstSelectedGameObject = newGameButton;
			EventSystem.SetSelectedGameObject(newGameButton);
	}

	public void NewGame()
	{
		_manager.StartNewGame();
	}

	public void Continue()
	{
		_manager.StartContinue();
	}

	public void Quit()
	{
		_manager.StartQuit();
	}
}
