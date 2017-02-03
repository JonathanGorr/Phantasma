using UnityEngine;
using System.Collections;

public class MotherMet : MonoBehaviour {

	private PlayerPreferences _prefs;
	private Transform _target;
	public float convoDistance = 2;

	void Start()
	{
		_target = GameObject.Find ("_Player").transform;
		_prefs = GameObject.Find("_LevelManager").GetComponent<PlayerPreferences>();
	}

	void Update()
	{
		float distance = Vector3.Distance (transform.position, _target.position);

		if(!_prefs.motherMet)
		{
			if(distance < convoDistance)
				_prefs.MotherMet ();
		}
	}
}
