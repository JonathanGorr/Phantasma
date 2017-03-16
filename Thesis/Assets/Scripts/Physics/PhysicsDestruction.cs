using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Physics destruction:
/// Used to break objects by gravity.
/// </summary>

public class PhysicsDestruction : MonoBehaviour {

	bool destroyed = false;
	public Rigidbody2D rbody;
	public Destructable destructable;
	float magnitude;
	public float breakTolerance = 3;

	void FixedUpdate()
	{
		if(destroyed) return;
		if(rbody.IsSleeping()) return;
		magnitude = rbody.velocity.sqrMagnitude;
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		if(destroyed) return;
		if(rbody.IsSleeping()) return;
		if(Mathf.Abs(magnitude) > breakTolerance * rbody.mass) 
		{
			destroyed = true;
			destructable.Explode(col.contacts[0].point); 
		}
	}
}
