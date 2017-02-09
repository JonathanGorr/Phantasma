using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

	[HideInInspector]
	public bool paused = false;

	[HideInInspector] public bool canTransition;
	private string currentSceneName;

	//components
	private Multiplayer.Avatar avatar;
	public Fading _fader;
	public Multiplayer _mp;
	public PlayerPreferences _prefs;
	public Evolution _evo;
	public PlayerInput _input;
	public UI ui;
	public EventSystem es;
	public SFX _sfx;

	public delegate void OnPause();
	public static event OnPause pause;
	public delegate void UnPause();
	public static event UnPause unPause;

	[HideInInspector]
	private bool controlOn;

	public Multiplayer.Avatar Avatar
	{
		get
		{
			if(avatar == null)
			{
				avatar = _mp.CreatePlayer();
			}
			return avatar;
		}
	}

	public Player Player
	{
		get
		{
			return Avatar.Player;
		}
	}

	void Awake()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode m)
	{
		Application.targetFrameRate = 60;
		currentSceneName = scene.name;

		if(scene.name == "Initialize")
		{
			Cursor.visible = false;
		}
		else if(scene.name == "Menu")
		{
			Cursor.visible = true;
		}
		else
		{
			_fader.BeginFade (-1);
			Cursor.visible = false;
		}
	}

	void Update()
	{
		if(currentSceneName != "Start") return;

		if(paused)
		{
			if(_input._roll)
			{
				Resume();
			}
		}
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
		else if(_input._select)
		{
			//toggle
			controlOn = !controlOn;

			if(controlOn)
			{
				ui.Reveal(ui._controlScreen);
			}
			else
			{
				ui.Hide(ui._controlScreen);
			}
		}
	}

	public void Pause()
	{
		StartCoroutine(IEPause());
	}
	private IEnumerator IEPause()
	{
		yield return new WaitForEndOfFrame();
		if(pause != null) pause();
		ui.ClearUpdates();
		if(controlOn) UI.TurnOff(ui._controlScreen);
		ui.Reveal(ui._pauseScreen);
		//set this button as the active selection
		if(!_input._controller) Cursor.visible = true;
		paused = true;

		//play sound then wait until sound finished to set volume
		_sfx.PlayUI("pause");
		while(_sfx.asrc.isPlaying) 
		{
			yield return null;
		}
		AudioListener.volume = .25f;
	}
	public void Resume()
	{
		StartCoroutine(IEResume());
	}
	public IEnumerator IEResume()
	{
		yield return new WaitForEndOfFrame();
		if(unPause != null) unPause();
		//resume if not paused
		_sfx.PlayUI("pause");
		AudioListener.volume = 1f;
		if(controlOn)UI.TurnOn(ui._controlScreen);
		ui.Hide(ui._pauseScreen);
		if(!_input._controller) Cursor.visible = false;
		paused = false;
		yield return null;
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
		_prefs.EraseAll();
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
		if(_prefs != null) _prefs.SaveStats (Player.transform.position, _evo.blood, Player.GetComponent<Health>().health);
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
		if(_prefs && !Player._health.dead)
		{
			//if a checkpoint was never met, set respawn to respawn
			if(PlayerPrefs.GetFloat("playerX") == 0 && PlayerPrefs.GetFloat("playerY") == 0)
				_prefs.SaveStats(_prefs.spawn.position, _evo.blood, Player.GetComponent<Health>().health);
				//else if a checkpoint was hit, set the respawn location to that
			else
				_prefs.SaveStats (new Vector3(PlayerPrefs.GetFloat("playerX"), PlayerPrefs.GetFloat("playerY"), 0), _evo.blood, Player.GetComponent<Health>().health);
		}
		//else, set their health to max and remove their blood, set location to prefs location
		else
		{
			_prefs.SaveStats (new Vector3(PlayerPrefs.GetFloat("playerX"),PlayerPrefs.GetFloat("playerY"),0), 0, Player.GetComponent<Health>().maxHealth);
		}

		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}