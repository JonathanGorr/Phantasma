using UnityEngine;
using System.Collections;

public class KillVolume : MonoBehaviour {

	private Camera2DFollow _camera;
	private DeathMessage _message;

	void Awake()
	{
		_message = GameObject.Find ("_LevelManager").GetComponent<DeathMessage>();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject.tag == "Player")
			_message.StartRestart();

		else if(other.gameObject.tag == "Enemy")
			Destroy(other);
	}
}
