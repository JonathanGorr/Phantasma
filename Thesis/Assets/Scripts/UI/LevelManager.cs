using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour {

	[HideInInspector]
	public static int bloodCount;
	public float timeLeft = 30f;
	public bool exhibition;

	[HideInInspector] public bool inMenu, canTransition;

	[SerializeField, HideInInspector]
	public GameObject _pauseScreen, _controlScreen, _hud, _dialog;
	private GameObject restartButton;

	//components
	private Health _player;
	private PlayerPreferences _prefs;
	private Evolution _evo;
	private Animator cameraAnim;
	private PlayerInput _input;

	[HideInInspector]
	public bool paused;
	private bool controlOn;

	private GameObject eventSystem;
	private EventSystem es;

	void Awake()
	{
		Application.targetFrameRate = 60;
		AudioListener.volume = 1f;

		//components
		_input = GetComponent<PlayerInput> ();
		_dialog = GameObject.Find("Dialog");
		cameraAnim = Camera.main.GetComponent<Animator> ();
		_hud = GameObject.Find ("Hud");
		_prefs = GetComponent<PlayerPreferences>();
		_evo = GetComponent<Evolution>();
		_player = GameObject.Find("_Player").GetComponent<Health>();
		_pauseScreen = GameObject.Find ("PauseMenu");
		_controlScreen = GameObject.Find ("ControlScreen");
		eventSystem = GameObject.Find ("EventSystem");
		restartButton = GameObject.Find ("RestartButton");

		//turn them off if existing
		if(_pauseScreen)
			_pauseScreen.SetActive (false);
		if(_controlScreen)
			_controlScreen.SetActive (false);

		if (Application.loadedLevelName == "Menu")
			inMenu = true;
		else
			inMenu = false;

		if(_hud)
		{
			if(inMenu)
			{
				Cursor.visible = true;
				_hud.SetActive(false);
			}
			else
			{
				Cursor.visible = false;
				_hud.SetActive(true);
			}
		}
	}

	void Update()
	{
		/*
		if(_dialog)
		{
			if(!ConversationManager.Instance.talking)
				_dialog.SetActive(false);
			else
				_dialog.SetActive(true);
		}
		*/

		//if no buttons are pressed, count down from timeLeft
		if(!inMenu)
		{
			if(exhibition)
			{
				if (!_input._anyKey)
					timeLeft -= Time.deltaTime; 
				else
					timeLeft = 30;

				//return to menu if x seconds passed
				if (timeLeft <= 0)
					ReturnToMenu ();
			}
		}

		if(!_player.dead)
		{
			if(!inMenu)
			{
				//pause
				if(_input._pause)
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
			else if(_input._anyKey) cameraAnim.SetTrigger("Skip");
		}
	}

	private void Pause()
	{
		//actually pause the game
		AudioListener.volume = .1f;
		_pauseScreen.SetActive (true);
		//set this button as the active selection
		eventSystem.GetComponent<EventSystem>().SetSelectedGameObject(restartButton);
		Cursor.visible = true;
		Time.timeScale = 0;
	}
	
	public void Resume()
	{
		//resume if not paused
		AudioListener.volume = 1f;
		_pauseScreen.SetActive (false);
		Time.timeScale = 1;
		Cursor.visible = false;
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
		if(_prefs && !_player.dead)
		{
			//if a checkpoint was never met, set respawn to respawn
			if(PlayerPrefs.GetFloat("playerX") == 0 && PlayerPrefs.GetFloat("playerY") == 0)
				_prefs.SaveStats(_prefs._spawn.position.x, _prefs._spawn.position.y, _evo.blood, _player.GetComponent<Health>().health);
				//else if a checkpoint was hit, set the respawn location to that
			else
				_prefs.SaveStats (PlayerPrefs.GetFloat("playerX"), PlayerPrefs.GetFloat("playerY"), _evo.blood, _player.GetComponent<Health>().health);
		}
		//else, set their health to max and remove their blood, set location to prefs location
		else
		{
			_prefs.SaveStats (PlayerPrefs.GetFloat("playerX"),PlayerPrefs.GetFloat("playerY"), 0, _player.GetComponent<Health>().maxHealth);
		}
		Application.LoadLevel(Application.loadedLevel);
	}
}