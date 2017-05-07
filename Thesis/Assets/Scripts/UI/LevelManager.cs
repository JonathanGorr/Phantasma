using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

	public static LevelManager Instance = null;

	[HideInInspector] public bool canTransition;
	//private string currentSceneName;

	//components
	private Multiplayer.Avatar avatar;
	public Fading _fader;
	public Multiplayer _mp;
	public Evolution _evo;
	public UI ui;
	public EventSystem es;
	public SFX _sfx;

	[HideInInspector]
	private bool controlOn;

	void Awake()
	{
		Time.timeScale = 1;
		DontDestroyOnLoad(this.gameObject);
		if(Instance == null) Instance = this;
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode m)
	{
		Application.targetFrameRate = 60;
		_fader.BeginFade (-1); //call the fade in function
		//currentSceneName = scene.name;

		if(scene.name == "Initialize")
		{
			//go to the next scene( Menu )
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
			Cursor.visible = false;
		}
		else if(scene.name == "Menu")
		{
			Cursor.visible = true;
		}
		else
		{
			Cursor.visible = false;
		}
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.F12))
		{
			CaptureScreenShot();
		}
	}

	int screenshotNumber;
	//used to create screenshots
	void CaptureScreenShot()
	{
		Application.CaptureScreenshot("Phantasma" + screenshotNumber.ToString() + ".png");
		screenshotNumber ++;
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

	public delegate void OnLevelChange();
	public static event OnLevelChange onLevelChange;

	public delegate void OnNewGame();
	public static event OnNewGame onNewGame;

	public IEnumerator NewGame()
	{
		PlayerPrefs.DeleteAll();
		Time.timeScale = 1;
		SaveSystem.DeleteSaveGame(0);
		SaveSystem.Save(0);
		if(onNewGame != null) onNewGame();
		if(onLevelChange != null) onLevelChange();
		yield return new WaitForSeconds (_fader.BeginFade (1));
		SceneManager.LoadScene(2);
	}

	public delegate void OnGoToMenu();
	public static event OnGoToMenu onGoToMenu;

	public IEnumerator GoToMenu()
	{
		SaveSystem.Save(0);
		Time.timeScale = 1;
		if(onGoToMenu != null) onGoToMenu();
		if(onLevelChange != null) onLevelChange();
		yield return new WaitForSeconds (_fader.BeginFade (1));
		SceneManager.LoadScene(1);
	}

	public delegate void OnContinue();
	public static event OnContinue onContinue;

	public IEnumerator Continue()
	{
		SaveSystem.Load(0);
		Time.timeScale = 1;
		if(onContinue != null) onContinue();
		if(onLevelChange != null) onLevelChange();
		yield return new WaitForSeconds (_fader.BeginFade (1));
		SceneManager.LoadScene(2);
	}

	public delegate void OnQuit();
	public static event OnQuit onQuit;

	public IEnumerator Quit()
	{
		SaveSystem.Save(0);
		Time.timeScale = 1;
		if(onQuit != null) onQuit();
		if(onLevelChange != null) onLevelChange();
		yield return new WaitForSeconds (_fader.BeginFade (1));
		Application.Quit ();
	}

	void OnApplicationQuit()
	{
		SaveSystem.Save(0);
	}

	public delegate void OnRestart();
	public static event OnRestart onRestart;

	public IEnumerator Restart()
	{
		//player is not dead, set position to last checkpoint
		if(Player.Instance._health.Dead)
		{
			SaveSystem.Load(0);
		}
		//else, set their health to max and remove their blood, set location to prefs location
		else
		{
			SaveSystem.Save(0);
		}

		Time.timeScale = 1;
		if(onRestart != null) onRestart();
		if(onLevelChange != null) onLevelChange();
		yield return new WaitForSeconds (_fader.BeginFade (1));

		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}