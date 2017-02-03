using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

	private Slider _slider;
	private GameObject _healthBar;
	private Health _health;
	private bool inMenu;

	void Start()
	{
		_health = GameObject.Find("_Player").GetComponent<Health> ();
		_healthBar = GameObject.Find ("HealthBar");
		_slider = _healthBar.GetComponent<Slider> ();
		_slider.minValue = 0;
		inMenu = GameObject.Find ("_LevelManager").GetComponent<LevelManager>().inMenu;

		_healthBar.SetActive(!inMenu);
	}
	
	// Update is called once per frame
	void Update () 
	{
		_slider.maxValue = _health.maxHealth;
		_slider.value = _health.health;
	}
}
