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

	void Start()
	{
		if(!SaveSystem.DoesSaveGameExist(0))
		{
			print("no save game");
		 	continueButton.SetActive(false);
	 	}
		SetMenu();

		Player.Instance.SetPosition(GameObject.FindWithTag("Respawn").transform.position);
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

	void SetMenu(string inP)
	{
		EventSystem.firstSelectedGameObject = newGameButton;
			EventSystem.SetSelectedGameObject(newGameButton);
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
