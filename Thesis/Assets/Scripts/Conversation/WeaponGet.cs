using UnityEngine;
using System.Collections;

public class WeaponGet : MonoBehaviour {

	private WeaponSwitcher _switcher;
	private FollowEntity _spirit;
	private PlayerPreferences _prefs;

	// Use this for initialization
	void Awake () {
		_switcher = GameObject.Find ("_LevelManager").GetComponent<WeaponSwitcher>();
		_spirit = GameObject.Find ("Father").GetComponent<FollowEntity>();
		_prefs = GameObject.Find("_LevelManager").GetComponent<PlayerPreferences>();
	}

	void OnTriggerEnter2D(Collider2D player)
	{
		if(_switcher != null && _spirit != null)
		{
			if(player.gameObject.tag == "Player")
			{
				_switcher.weaponGet = true;
				_prefs.ItemsGet();
				_spirit.spiritGet = true;
			}
		}
	}
}
