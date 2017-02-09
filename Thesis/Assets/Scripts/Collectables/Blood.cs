using UnityEngine;
using System.Collections;

//blood sprite object derives from collectible
public class Blood : Collectable {
	
	private Evolution _evo;

	//how much it gives to the player score
	private int blood = 1;

	void OnEnable()
	{
		_evo = GameObject.Find("_LevelManager").GetComponent<Evolution> ();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "Player")
		{
			_evo.AddBlood(blood);
			Destroy(this.gameObject);
		}
	}
}
