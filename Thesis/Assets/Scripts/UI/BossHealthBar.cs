using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour {
	
	private Slider _slider;
	private Health _health;
	private bool inMenu;
	private GameObject _victoryScreen;
	private LevelManager _manager;
	private PlayerPreferences _prefs;
	
	[HideInInspector]
	public bool bossFight = false;
	private bool called;
	
	private Animator _victoryAnim;
	private Animator _gameOver;
	
	// Use this for initialization
	void Awake() {
		//health
		_health = GameObject.Find("Boss").GetComponent<Health>();
		_manager = GetComponentInParent<LevelManager>();
		_prefs = GetComponentInParent<PlayerPreferences>();
		_gameOver = GameObject.Find("GameOver").GetComponent<Animator>();
		//victory Screen
		_victoryScreen = GameObject.Find("VictoryScreen");
		_victoryAnim = _victoryScreen.GetComponent<Animator>();
		//bar
		_slider = GetComponent<Slider> ();
		_slider.minValue = 0;
		//in menu?
		inMenu = GameObject.Find ("_LevelManager").GetComponent<LevelManager>().inMenu;
		
		if (inMenu)
			gameObject.SetActive (false);

		_gameOver.SetInteger("AnimState", 0);
	}
	
	void FixedUpdate()
	{
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
		while (!Input.GetButtonDown("360_AButton"))
			yield return null;
	}
}
