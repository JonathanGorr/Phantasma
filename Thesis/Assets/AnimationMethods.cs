using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationMethods : MonoBehaviour {

	private SFX _sfx;
	public bool attacking = false;
	public bool canFire = true;

	void Start()
	{
		_sfx = GameObject.Find("_LevelManager").GetComponent<SFX>();
	}

	public void SwingSoundLight()
	{
		_sfx.PlayFX("swing_Light", transform.position);
	} 

	public void SwingSoundHeavy()
	{
		_sfx.PlayFX("swing_Heavy", transform.position);
	}

	public void CanFire()
	{
		canFire = true;
	}

	public void CantFire()
	{
		canFire = false;
	}

	public void Attacking()
	{
		attacking = true;
	}

	public void NotAttacking()
	{
		attacking = false;
	}
}
