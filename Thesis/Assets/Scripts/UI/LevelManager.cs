using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

	[HideInInspector]
	public bool paused = false;
	public int bloodCount;
	public float timeLeft = 30f;
	public bool exhibition;

	[HideInInspector] public bool inMenu = false, inInitialize = true, canTransition;

	public GameObject _pauseScreen;
	public GameObject  _controlScreen;
	public GameObject  _hud;
	public GameObject  _dialog;
	private GameObject restartButton;

	//components
	private Health _player;
	private Fading _fader;
	private PlayerPreferences _prefs;
	private Evolution _evo;
	private Animator cameraAnim;
	private PlayerInput _input;

	[HideInInspector]
	private bool controlOn;

	private GameObject eventSystem;
	private EventSystem es;

	void Start()
	{
		DontDestroyOnLoad(this.gameObject);
		SceneManager.sceneLoaded += OnSceneLoaded;
		Application.targetFrameRate = 60;
		AudioListener.volume = 1f;

		_fader = GetComponent<Fading>();
		_input = GetComponent<PlayerInput> ();
		cameraAnim = Camera.main.GetComponent<Animator> ();
		_prefs = GetComponent<PlayerPreferences>();
		_evo = GetComponent<Evolution>();
		cameraAnim = Camera.main.GetComponent<Animator> ();
		_prefs = GetComponent<PlayerPreferences>();
		_evo = GetComponent<Evolution>();
		_player = GameObject.Find("_Player").GetComponent<Health>();
		eventSystem = GameObject.Find ("EventSystem");
		restartButton = GameObject.Find ("RestartButton");
		_hud.SetActive(false);
		//turn them off if existing
		_pauseScreen.SetActive (false);
		_controlScreen.SetActive (false);
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode m)
	{
		if(scene.name == "Menu")
		{
			inMenu = true;
			inInitialize = false;
			Cursor.visible = true;
		}
		else if(scene.name == "Initialize")
		{
			inInitialize = true;
			inMenu = false;
			Cursor.visible = false;
		}
		else
		{
			_fader.BeginFade (-1);
			inInitialize = false;
			inMenu = false;
			Cursor.visible = false;
			_hud.SetActive(true);
		}
	}

	void Update()
	{
		if(inMenu) return;
		if(inInitialize) return;

		if(exhibition)
		{
			if (!_input._anyKeyDown)
				timeLeft -= Time.deltaTime; 
			else
				timeLeft = 30;

			//return to menu if x seconds passed
			if (timeLeft <= 0)
				ReturnToMenu ();
		}

		if(!_player.dead)
		{
			if(!inMenu)
			{
				//pause
				if(paused)
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
				else
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
			else if(_input._anyKeyDown) cameraAnim.SetTrigger("Skip");
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
		float fadeTime = GetComponent<Fading> ().BeginFade (1);
		yield return new WaitForSeconds (fadeTime);
		SceneManager.LoadScene(2);
	}

	public IEnumerator GoToMenu()
	{
		Time.timeScale = 1;
		//load level
		paused = false;
		float fadeTime = _fader.BeginFade (1);
		yield return new WaitForSeconds (fadeTime);
		if(_prefs != null)
			_prefs.SaveStats (_player.transform.position, _evo.blood, _player.GetComponent<Health>().health);
		SceneManager.LoadScene(0);
	}

	public IEnumerator Continue()
	{
		Time.timeScale = 1;
		//load level
		paused = false;
		float fadeTime = _fader.BeginFade (1);
		yield return new WaitForSeconds (fadeTime);
		SceneManager.LoadScene(2);
	}

	public IEnumerator Quit()
	{
		Time.timeScale = 1;
		paused = false;
		float fadeTime = _fader.BeginFade (1);
		yield return new WaitForSeconds (fadeTime);
		Application.Quit ();
	}

	public IEnumerator Restart()
	{
		Time.timeScale = 1;
		//if player is not dead, save their stats
		paused = false;
		float fadeTime = _fader.BeginFade (1);
		yield return new WaitForSeconds (fadeTime);

		//player is not dead, set position to last checkpoint
		if(_prefs && !_player.dead)
		{
			//if a checkpoint was never met, set respawn to respawn
			if(PlayerPrefs.GetFloat("playerX") == 0 && PlayerPrefs.GetFloat("playerY") == 0)
				_prefs.SaveStats(_prefs._spawn.position, _evo.blood, _player.GetComponent<Health>().health);
				//else if a checkpoint was hit, set the respawn location to that
			else
				_prefs.SaveStats (new Vector3(PlayerPrefs.GetFloat("playerX"), PlayerPrefs.GetFloat("playerY"), 0), _evo.blood, _player.GetComponent<Health>().health);
		}
		//else, set their health to max and remove their blood, set location to prefs location
		else
		{
			_prefs.SaveStats (new Vector3(PlayerPrefs.GetFloat("playerX"),PlayerPrefs.GetFloat("playerY"),0), 0, _player.GetComponent<Health>().maxHealth);
		}

		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}