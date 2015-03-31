using UnityEngine;
using System.Collections;

public class MotherMet : MonoBehaviour {

	private PlayerPreferences _prefs;

	void Awake()
	{
		_prefs = GameObject.Find("_LevelManager").GetComponent<PlayerPreferences>();
	}

	void OnTriggerEnter2D(Collider2D player)
	{
		if (player.gameObject.tag == "Player")
			_prefs.MotherMet ();
	}
}
