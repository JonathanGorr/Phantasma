using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DeathMessage : WaitForPlayer {

	private GameObject deathMessage;
	private Health _health;
	public float delay = 3f;

	public override IEnumerator Initialize(UnityEngine.SceneManagement.Scene scene)
	{
		while(_manager.Player == null) yield return null;

		_health = _manager.Player.GetComponent<Health>();
		deathMessage = GameObject.Find ("DeathMessage");
		if(deathMessage) deathMessage.SetActive (false);
	}

	void Update()
	{
		if(!_health) return;

		if(_health.dead)
		{
			StartCoroutine(Restart());
		}
	}

	public void StartRestart()
	{
		StartCoroutine ("Restart");
	}

	public IEnumerator Restart()
	{
		Camera.main.GetComponent<CameraController>().enabled = false;
		deathMessage.SetActive (true);
		yield return new WaitForSeconds(delay);
		deathMessage.SetActive (false);
		_manager.StartRestart ();
	}
}
