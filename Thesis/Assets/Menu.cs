using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour {

	LevelManager _manager;

	void Start()
	{
		_manager = GameObject.Find("_LevelManager").GetComponent<LevelManager>();
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
