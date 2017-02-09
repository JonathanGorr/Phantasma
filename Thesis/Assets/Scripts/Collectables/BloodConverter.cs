using UnityEngine;
using System.Collections;

public class BloodConverter : WaitForPlayer {

	private Health _health;
	private Evolution _evo;
	private bool _convert;
	public AudioClip _healClip;
	private PlayerInput _input;

	public override IEnumerator Initialize(UnityEngine.SceneManagement.Scene scene)
	{
		_evo = GameObject.Find("_LevelManager").GetComponent<Evolution>();
		_input = GameObject.Find ("_LevelManager").GetComponent<PlayerInput> ();

		while(_manager.Player == null) yield return null;

		_health = _manager.Player.GetComponent<Health>();
	}

	void Update()
	{
		if(!_health) return;

		//if health is not max, and we have at least 1 blood, convert
		if(_health.health < _health.maxHealth && _evo.blood > 0)
		{
			if(_input._heal)
			{
				_evo.SubtractBlood(1);
				_health.Heal(1);
				GetComponent<AudioSource>().PlayOneShot(_healClip);
			}
		}
	}
}
