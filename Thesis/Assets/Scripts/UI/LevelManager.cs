﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour {

	[HideInInspector]
	public static int bloodCount;

	public bool inMenu;
	private bool skip;

	[SerializeField, HideInInspector]
	public GameObject _pauseScreen, _controlScreen, _hud;

	private Health _player;
	private PlayerPreferences _prefs;
	private Evolution _evo;
	private Animator cameraAnim;
	private GameObject pauseButton, menuButton;

	[HideInInspector]
	public bool paused = false;
	private bool controlOn, pause;

	private GameObject eventSystem;
	private EventSystem es;

	void Awake()
	{
		Application.targetFrameRate = 60;

		cameraAnim = Camera.main.GetComponent<Animator> ();
		_hud = GameObject.Find ("Hud");
		_prefs = GetComponent<PlayerPreferences>();
		_evo = GetComponent<Evolution>();
		_player = GameObject.Find("_Player").GetComponent<Health>();
		//find controller and pause screen
		_pauseScreen = GameObject.Find ("PauseMenu");
		_controlScreen = GameObject.Find ("ControlScreen");
		eventSystem = GameObject.Find ("EventSystem");
		//es = eventSystem.GetComponent<EventSystem> ();
		menuButton = GameObject.Find ("New Game");
		pauseButton = GameObject.Find ("Restart");

		//turn them off if existing
		if(_pauseScreen != null)
			_pauseScreen.SetActive (false);
		if(_controlScreen != null)
			_controlScreen.SetActive (false);

		if(_hud != null)
		{
			if(inMenu)
			{
				_hud.SetActive(false);
			}
			else
				_hud.SetActive(true);
		}
	}

	void Update()
	{
		//input
		pause = Input.GetButtonDown ("360_StartButton") || Input.GetKeyDown (KeyCode.F);
		skip = Input.anyKeyDown;

		if(!_player.dead)
		{
			if(!inMenu)
			{
				//pause
				if(pause)
				{
					//toggle
					paused = !paused;

					if(paused)
					{
						Pause();
					}
					else
					{
						Resume();
					}
				}

				//cannot open control menu when paused
				if(!paused)
				{
					//control
					if(Input.GetButtonDown("360_BackButton") && !inMenu)
					{
						//toggle
						controlOn = !controlOn;

						if(controlOn)
						{
							_controlScreen.SetActive (true);
						}
						else
						{
							_controlScreen.SetActive (false);
						}

						_controlScreen.SetActive (controlOn);
					}
				}
			}

			//else if in menu
			else
			{
				if(skip)
					cameraAnim.SetTrigger("Skip");

				//eventSystem.GetComponent<EventSystem>().firstSelectedGameObject = menuButton;
			}
		}
	}

	private void Pause()
	{
		//actually pause the game
		_pauseScreen.SetActive (true);
		//set this button as the active selection
		//eventSystem.GetComponent<EventSystem>().firstSelectedGameObject = pauseButton;
		Time.timeScale = 0;
	}
	
	public void Resume()
	{
		//resume if not paused
		_pauseScreen.SetActive (false);
		Time.timeScale = 1;
	}

	public void AddBlood(int blood)
	{
		bloodCount += blood;
	}

	public void StartNewGame ()
	{
		StartCoroutine (NewGame());
	}

	public void StartContinue()
	{
		StartCoroutine (Continue());
	}

	public void StartQuit()
	{
		StartCoroutine (Quit());
	}

	public void ReturnToMenu()
	{
		StartCoroutine (GoToMenu());
	}

	public void StartRestart ()
	{
		StartCoroutine (Restart());
	}

public IEnumerator NewGame()
	{
		Time.timeScale = 1;
		//load level
		paused = false;
		float fadeTime = GameObject.Find ("_LevelManager").GetComponent<Fading> ().BeginFade (1);
		yield return new WaitForSeconds (fadeTime);
		Application.LoadLevel(1);
	}

public IEnumerator GoToMenu()
	{
		Time.timeScale = 1;
		//load level
		paused = false;
		float fadeTime = GameObject.Find ("_LevelManager").GetComponent<Fading> ().BeginFade (1);
		yield return new WaitForSeconds (fadeTime);
		if(_prefs != null)
			_prefs.SaveStats (_player.transform.position.x, _player.transform.position.y, _evo.blood, _player.GetComponent<Health>().health);
		Application.LoadLevel(0);
	}

public IEnumerator Continue()
	{
		Time.timeScale = 1;
		//load level
		paused = false;
		float fadeTime = GameObject.Find ("_LevelManager").GetComponent<Fading> ().BeginFade (1);
		yield return new WaitForSeconds (fadeTime);
		Application.LoadLevel(1);
	}

public IEnumerator Quit()
	{
		Time.timeScale = 1;
		paused = false;
		float fadeTime = GameObject.Find ("_LevelManager").GetComponent<Fading> ().BeginFade (1);
		yield return new WaitForSeconds (fadeTime);
		Application.Quit ();
	}

public IEnumerator Restart()
	{
		Time.timeScale = 1;
		//if player is not dead, save their stats
		paused = false;
		float fadeTime = GameObject.Find ("_LevelManager").GetComponent<Fading> ().BeginFade (1);
		yield return new WaitForSeconds (fadeTime);

		//player is not dead, set position to last checkpoint
		if(_prefs != null && !_player.dead)
		{
			//if a checkpoint was never met, set respawn to respawn
			if(PlayerPrefs.GetFloat("playerX") == 0 && PlayerPrefs.GetFloat("playerY") == 0)
				_prefs.SaveStats(_prefs._spawn.position.x, _prefs._spawn.position.y, _evo.blood, _player.GetComponent<Health>().health);
				//else if a checkpoint was hit, set the respawn location to that
			else
				_prefs.SaveStats (PlayerPrefs.GetFloat("playerX"),PlayerPrefs.GetFloat("playerY"), _evo.blood, _player.GetComponent<Health>().health);
		}
		//else, set their health to max and remove their blood, set location to prefs location
		else
		{
			_prefs.SaveStats (PlayerPrefs.GetFloat("playerX"),PlayerPrefs.GetFloat("playerY"), 0, _player.GetComponent<Health>().maxHealth);
		}
		Application.LoadLevel(Application.loadedLevel);
	}
}