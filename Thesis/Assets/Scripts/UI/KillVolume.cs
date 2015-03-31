using UnityEngine;
using System.Collections;

public class KillVolume : MonoBehaviour {

	private GameObject _manager;
	private LevelManager _managerComp;
	private GameObject deathMessage;
	private Camera2DFollow _camera;

	void Awake()
	{
		_manager = GameObject.Find ("_LevelManager");
		_managerComp = _manager.GetComponent<LevelManager>();
		deathMessage = GameObject.Find ("DeathMessage");
		_camera = GameObject.Find ("_MainCamera").GetComponent<Camera2DFollow>();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject.tag == "Player")
		{
			StartCoroutine(Kill());
		}
		else if(other.gameObject.tag == "Enemy")
			Destroy(other);
	}

	private IEnumerator Kill()
	{
		deathMessage.SetActive(true);
		_camera.enabled = false;
		yield return new WaitForSeconds (2f);
		_managerComp.StartRestart();
	}
}
