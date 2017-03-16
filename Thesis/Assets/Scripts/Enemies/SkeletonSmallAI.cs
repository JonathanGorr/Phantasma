using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonSmallAI : EnemyAI {

	public AudioSource ratlingBones;
	SlideAttackStateBehaviour slideAttackState;

	public float reposteChance = .33f;

	public override void Awake ()
	{
		base.Awake ();
		animMethods.onCollapse += CollapseParticles;
	}

	public override void Subscribe()
	{
		slideAttackState = _anim.GetBehaviour<SlideAttackStateBehaviour>();
		slideAttackState.onEnter += SlideAttackEnter;
		slideAttackState.onExit += SlideAttackExit;
		animMethods.onSlide += SlideAttack;

		base.Subscribe();
	}

	public override void UnSubscribe()
	{
		slideAttackState.onEnter -= SlideAttackEnter;
		slideAttackState.onExit -= SlideAttackExit;
		animMethods.onSlide -= SlideAttack;

		base.UnSubscribe();
	}

	public void SlideAttackEnter()
	{
		SetDamping(actionDamping);
		canMove = false;
		canFace = false;
	}
	public void SlideAttackExit()
	{
		SetDamping(groundDamping);
		canMove = true;
		canFace = true;
	}

	public override void SlideAttack()
	{
		if(CanForce()) Force(slidingAttack);
		base.SlideAttack();
	}

	public override void OnBackstepExit()
	{
		base.OnBackstepExit();

		//resposte sometimes
		if(Random.value < reposteChance)
		{
			_anim.SetTrigger("SlidingAttack");
		}
	}

	public override void OnAttackEnter()
	{
		SFX.Instance.PlayFX("windup_Light", myTransform.position);
		base.OnAttackEnter();
	}

	public override void OnHurt (Entity offender)
	{
		SFX.Instance.PlayFX("hurt_skeleton", myTransform.position);
		base.OnHurt (offender);
	}

	void CollapseParticles()
	{
		_footDust.Emit(10);
	}

	public override void OnDeath()
	{
		ratlingBones.Stop();
		SFX.Instance.PlayFX("death_skeleton", myTransform.position);
		base.OnDeath();
	}
}
