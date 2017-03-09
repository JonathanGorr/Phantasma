using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationMethods : MonoBehaviour {

	public bool attacking = false;
	public bool canFire = true;

	//when the weapon is supposed to pull or lunge the entity forward due to it's weight and momentum
	public delegate void OnLunge();
	public event OnLunge onLunge;

	//when the weapon is supposed to pull or lunge the entity forward due to it's weight and momentum
	public delegate void OnLeap();
	public event OnLeap onLeap;

	//when the weapon is supposed to pull or lunge the entity forward due to it's weight and momentum
	public delegate void OnSlide();
	public event OnSlide onSlide;

	public delegate void OnCollapse();
	public event OnCollapse onCollapse;

	public delegate void OnBackstep();
	public event OnBackstep onBackstep;

	public void SwingSoundLight()
	{
		if(onLunge != null) onLunge();
		SFX.Instance.PlayFX("swing_Light", transform.position);
	}

	public void DropAttack()
	{
		attacking = true;
		SFX.Instance.PlayFX("swing_Heavy", transform.position);
	}

	public void Backstep()
	{
		if(onBackstep != null) onBackstep();
	}

	public void Collapse()
	{
		if(onCollapse != null) onCollapse();
	}

	public void LeapAttack()
	{
		if(onLeap != null) onLeap();
	}

	public void Destroy()
	{
		Destroy(this.gameObject);
	}

	public void SlideAttack()
	{
		if(onSlide != null) onSlide();
	}

	public void SwingSoundHeavy()
	{
		if(onLunge != null) onLunge();
		SFX.Instance.PlayFX("swing_Heavy", transform.position);
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
