using UnityEngine;
using System.Collections;

public class BloodConverter : MonoBehaviour {

	private Health _health;
	private Evolution _evo;
	private bool _convert;
	public AudioClip _healClip;

	void Awake()
	{
		_health = GameObject.Find("_Player").GetComponent<Health>();
		_evo = GameObject.Find("_LevelManager").GetComponent<Evolution>();
	}

	//handle input
	void Update()
	{
		_convert = Input.GetKeyDown (KeyCode.Q) || Input.GetButtonDown ("360_YButton");
	}

	void FixedUpdate()
	{
		//if health is not max, and we have at least 1 blood, convert
		if(_health.health < _health.maxHealth && _evo.blood > 0)
		{
			if(_convert)
			{
				_evo.SubtractBlood(1);
				_health.Heal(1);
				GetComponent<AudioSource>().PlayOneShot(_healClip);
			}
		}
	}
}
