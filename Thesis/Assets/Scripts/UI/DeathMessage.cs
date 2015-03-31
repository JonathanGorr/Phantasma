using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DeathMessage : MonoBehaviour {

	private GameObject deathMessage;
	private Health _health;
	public float delay = 3f;
	private LevelManager _manager;

	void Awake()
	{
		_health = GameObject.Find("_Player").GetComponent<Health>();
		_manager = GetComponent<LevelManager> ();
		deathMessage = GameObject.Find ("DeathMessage");

		if(deathMessage != null)
			deathMessage.SetActive (false);
	}

	void FixedUpdate()
	{
		if(_health.dead)
		{
			StartCoroutine(Restart());
		}
	}

	public IEnumerator Restart()
	{
		deathMessage.SetActive (true);
		yield return new WaitForSeconds(delay);
		deathMessage.SetActive (false);
		_manager.StartRestart ();
	}
}
