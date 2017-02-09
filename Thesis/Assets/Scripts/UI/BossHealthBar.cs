using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BossHealthBar : MonoBehaviour {

	private LevelManager _manager;
	private Slider _slider;
	private GameObject boss;
	private Health _health;
	private GameObject _victoryScreen;
	private PlayerPreferences _prefs;
	private PlayerInput _input;
	
	[HideInInspector]
	public bool bossFight = false;
	private bool called;
	
	private Animator _victoryAnim;
	private Animator _gameOver;

	void Awake()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode m)
	{
		_manager = GameObject.Find("_LevelManager").GetComponent<LevelManager>();

		if(scene.name == "Menu")
		{
			gameObject.SetActive (false);
		}
		else if(scene.name == "Start")
		{
			//health
			boss = GameObject.Find ("Boss");
			_health = boss.GetComponent<Health>();
			_input = GetComponentInParent<PlayerInput> ();
			_prefs = GetComponentInParent<PlayerPreferences>();
			_gameOver = GameObject.Find("GameOver").GetComponent<Animator>();
			//victory Screen
			_victoryScreen = GameObject.Find("VictoryScreen");
			_victoryAnim = _victoryScreen.GetComponent<Animator>();
			//bar
			_slider = GetComponent<Slider> ();
			_slider.minValue = 0;
			gameObject.SetActive (true);
			_gameOver.SetInteger("AnimState", 0);
		}
	}
	
	void FixedUpdate()
	{
		if(!_slider || !_health) return;
		_slider.maxValue = _health.maxHealth;
		_slider.value = _health.health;

		//if boss is dead do stuff
		if(_health.health <= 0 && !called)
		{
			_victoryAnim.SetTrigger("YouDefeated");
			StartCoroutine(ReturnToMenu());
		}
	}

	IEnumerator ReturnToMenu()
	{
		//stop update from calling this
		called = true;

		//wait for victory anim to finish
		yield return new WaitForSeconds (4f);

		//say thanks
		_gameOver.SetInteger("AnimState", 1);

		yield return StartCoroutine(WaitForButton());

		//hide thanks
		_gameOver.SetInteger("AnimState", 0);

		yield return new WaitForSeconds (1f);
		_prefs.EraseAll();
		_manager.ReturnToMenu();
	}
	
	//while the button is not pressed, stuck in a loop
	IEnumerator WaitForButton()
	{
		while (!_input._strongAttack)
			yield return null;
	}
}
