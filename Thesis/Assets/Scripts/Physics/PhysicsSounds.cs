using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsSounds : MonoBehaviour {

	public bool debug = false;
	public Rigidbody2D rbody;
	private SFX _sfx;
	public string bounce_sfx;
	public string drop_sfx;
	bool canBounce = true;
	float magnitude;

	void OnEnable()
	{
		if(!_sfx) _sfx = GameObject.Find("_LevelManager").GetComponent<SFX>();
	}

	void FixedUpdate()
	{
		if(!canBounce) return;
		if(rbody.IsSleeping()) return;
		magnitude = rbody.velocity.sqrMagnitude;
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		if(col.collider == null) return;
		if(!canBounce) return;
		if(debug) print(magnitude * rbody.mass);
		if((Mathf.Abs(magnitude)* rbody.mass) > .1f) 
		{
			_sfx.PlayFX(bounce_sfx, col.contacts[0].point); 
		}
		else
		{
			canBounce = false;
			_sfx.PlayFX(drop_sfx, col.contacts[0].point);
		}
	}
}
