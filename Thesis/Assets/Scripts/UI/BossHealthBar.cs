using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour {

	public Slider _slider;
	public Health _bossHealth;
	
	private GameObject _victoryScreen;
	private Animator _victoryAnim;
	private CanvasGroup _gameOver;

	void Awake()
	{
		//health
		_bossHealth.onDeath += Return;
		_gameOver = GameObject.Find("GameOver").GetComponent<CanvasGroup>();
		//victory Screen
		_victoryScreen = GameObject.Find("VictoryScreen");
		_victoryAnim = _victoryScreen.GetComponent<Animator>();
	}
	
	void FixedUpdate()
	{
		_slider.maxValue = _bossHealth.maxHealth;
		_slider.value = _bossHealth.health;
	}

	void Return()
	{
		_victoryAnim.SetTrigger("YouDefeated");
		StartCoroutine(ReturnToMenu());
	}

	IEnumerator ReturnToMenu()
	{
		//wait for victory anim to finish
		yield return new WaitForSeconds (4f);

		//say thanks
		Utilities.Instance.Reveal(_gameOver);

		while(!PlayerInput.Instance.ADown) yield return null;

		//hide thanks
		Utilities.Instance.Hide(_gameOver);

		yield return new WaitForSeconds (1f);
		PlayerPreferences.Instance.EraseAll();

		LevelManager.Instance.ReturnToMenu();
	}
}
