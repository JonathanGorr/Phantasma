using UnityEngine;
using System.Collections;

public class WeaponGet : MonoBehaviour {

	private WeaponSwitcher _switcher;
	private FollowEntity _spirit;
	private PlayerPreferences _prefs;
	private Transform _target;
	public float convoDistance = 2f;
	private bool done;

	// Use this for initialization
	void Awake () {
		_target = GameObject.Find ("_Player").transform;
		_switcher = GameObject.Find ("_LevelManager").GetComponent<WeaponSwitcher>();
		_spirit = GameObject.Find ("Father").GetComponent<FollowEntity>();
		_prefs = GameObject.Find("_LevelManager").GetComponent<PlayerPreferences>();
	}
	
	void Update()
	{
		float distance = Vector3.Distance (transform.position, _target.position);

		if(!done)
		{
			if(distance < convoDistance)
			{
				if(_switcher && _spirit)
				{
					_prefs.ItemsGet();
					_spirit.spiritGet = true;
					done = true;
				}
			}
		}
	}
}