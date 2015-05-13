using UnityEngine;
using System.Collections;

public class BossHead : MonoBehaviour {

	private Health _BossHealth;
	//private Transform _neck;
	//private Transform myTransform;
	private FlamingSkullAI _ai;

	void Awake()
	{
		_BossHealth = GetComponentInParent<Health>();
		//_neck = GameObject.Find("Neck").GetComponent<Transform>();
		_ai = GetComponent<FlamingSkullAI>();

		if (_ai != null)
			_ai.enabled = false;

		//cache it
		//myTransform = transform;
	}

	void FixedUpdate()
	{
		//if the bosses health is half gone
		if(_BossHealth.health <= _BossHealth.health/2)
		{
			_ai.enabled = true;
			print("boss health half gone");
		}

		//bind the head to the neck
		else
		{
			//myTransform = _neck;
		}

	}
}
