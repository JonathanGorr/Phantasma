using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BossHealthBar : MonoBehaviour {

	private Slider _slider;
	private GameObject boss;
	private Health _health;
	private GameObject _victoryScreen;
	bool returnToMenu = false;
	
	[HideInInspector]
	public bool bossFight = false;
	private bool called;
	
	private Animator _victoryAnim;
	private CanvasGroup _gameOver;

	void Awake()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode m)
	{
		if(scene.name == "Menu")
		{
			gameObject.SetActive (false);
		}
		else if(scene.name == "Start")
		{
			print("start");
			//health
			boss = GameObject.Find ("Boss");
			_health = boss.GetComponent<Health>();
			_health.onDeath += Return;
			_gameOver = GameObject.Find("GameOver").GetComponent<CanvasGroup>();
			//victory Screen
			_victoryScreen = GameObject.Find("VictoryScreen");
			_victoryAnim = _victoryScreen.GetComponent<Animator>();
			gameObject.SetActive (true);
		}
	}
	
	void FixedUpdate()
	{
		if(!_slider || !_health) return;
		_slider.maxValue = _health.maxHealth;
		_slider.value = _health.health;
	}

	void Return()
	{
		_victoryAnim.SetTrigger("YouDefeated");
		StartCoroutine(ReturnToMenu());
	}

	IEnumerator ReturnToMenu()
	{
		print("return to menu?");

		//wait for victory anim to finish
		yield return new WaitForSeconds (4f);

		//say thanks
		Utilities.Instance.Reveal(_gameOver);

		while(!PlayerInput.a) yield return null;

		//hide thanks
		Utilities.Instance.Hide(_gameOver);

		yield return new WaitForSeconds (1f);
		PlayerPreferences.Instance.EraseAll();

		LevelManager.Instance.ReturnToMenu();
	}
}
