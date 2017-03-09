using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Head Tracking:
/// Used by enemies to look at player.
/// Maybe eventually used by player to look at enemies or important things?
/// </summary>

public class HeadTracking : MonoBehaviour {

	public Health health;
	public Transform myTransform;
	public EnemyAI ai;

	void Awake()
	{
		if(!myTransform) myTransform = transform;
		health.onDeath += Disable;
	}

	void Disable()
	{
		this.enabled = false;
	}

	void LateUpdate()
	{
		if(ai.target == null) return;
		myTransform.LookAtTarget(ai.target.head);
	}
}
