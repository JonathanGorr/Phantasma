using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

	private Slider _slider;
	private GameObject _healthBar;
	private Health _health;
	private bool inMenu;

	// Use this for initialization
	void Awake() {
		_health = GameObject.Find ("_Player").GetComponent<Health> ();
		_healthBar = GameObject.Find ("HealthBar");
		_slider = _healthBar.GetComponent<Slider> ();
		_slider.minValue = 0;
		inMenu = GameObject.Find ("_LevelManager").GetComponent<LevelManager>().inMenu;

		if(inMenu)
		{
			_healthBar.SetActive(false);
		}
		else
			_healthBar.SetActive(true);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		_slider.maxValue = _health.maxHealth;
		_slider.value = _health.health;
	}
}
