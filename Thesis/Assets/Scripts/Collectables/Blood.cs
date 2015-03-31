using UnityEngine;
using System.Collections;

//blood sprite object derives from collectible
public class Blood : Collectable {
	
	private Evolution _evo;
	[SerializeField, HideInInspector]
	public GameObject _levelManager;
	private LevelManager _manager;

	//how much it gives to the player score
	private int blood = 1;

	void Start()
	{
		_levelManager = GameObject.Find ("_LevelManager");
		_manager = _levelManager.GetComponent<LevelManager> ();
		_evo = _levelManager.GetComponent<Evolution> ();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "Player")
		{
			_evo.AddBlood(blood);
			_manager.AddBlood(blood);
			Destroy(this.gameObject);
		}
	}
}
