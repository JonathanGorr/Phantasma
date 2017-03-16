using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

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
				_eventSystem = LevelManager.Instance.es;
			}
			return _eventSystem;
		}
	}

	void Awake()
	{
		PlayerInput.onSwitch += SetMenu;
	}

	void OnDisable()
	{
		PlayerInput.onSwitch -= SetMenu;
	}

	void Start()
	{
		//if(!SaveSystem.DoesSaveGameExist(0))
		if(PlayerPrefs.GetInt("GameCreated") == 0)
		{
		 	continueButton.SetActive(false);
	 	}
		SetMenu(null);

		Player.Instance.SetPosition(GameObject.FindWithTag("Respawn").transform.position);
	}

	//called whenever unity application window loses focus
	void OnApplicationFocus(bool hasFocus)
	{
		if(hasFocus)
		{
			SetMenu(null);
		}
	}

	void SetMenu(string input)
	{
		//set default button to continue or new game based on the save game state
		if(PlayerPrefs.GetInt("GameCreated") == 0)
		{
			EventSystem.firstSelectedGameObject = newGameButton;
			EventSystem.SetSelectedGameObject(newGameButton);
		}
		else
		{
			EventSystem.firstSelectedGameObject = continueButton;
			EventSystem.SetSelectedGameObject(continueButton);
		}
	}

	public void NewGame()
	{
		LevelManager.Instance.StartNewGame();
	}

	public void Continue()
	{
		LevelManager.Instance.StartContinue();
	}

	public void Quit()
	{
		LevelManager.Instance.StartQuit();
	}
}
