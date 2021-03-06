﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : EnemyAI {

	SlideAttackStateBehaviour slideAttackState;

	float tooCloseRange = 3f;

	public override void Subscribe()
	{
		base.Subscribe();

		if(_anim.GetBehaviour<SlideAttackStateBehaviour>())
		{
			slideAttackState = _anim.GetBehaviour<SlideAttackStateBehaviour>();
			slideAttackState.onEnter += OnSlideAttackEnter;
			slideAttackState.onExit += OnSlideAttackExit;
		}
	}

	public override void UnSubscribe()
	{
		base.UnSubscribe();

		if(slideAttackState)
		{
			slideAttackState.onEnter -= OnSlideAttackEnter;
			slideAttackState.onExit -= OnSlideAttackExit;
		}
	}

	void OnSlideAttackEnter()
	{
		SetDamping(actionDamping);
		canMove = false;
		canFace = false;
	}

	void OnSlideAttackExit()
	{
		SetDamping(groundDamping);
		canMove = true;
		canFace = true;
	}

	public override Facing Facing {
		get {
			return base.Facing;
		}
		set {
			if(facing == value) return;
			if(!canFace) return;
			facing = value;
			transform.localScale = new Vector2(facing == Facing.right ? -Mathf.Abs(startScale.x) : Mathf.Abs(startScale.x), startScale.y);
		}
	}

	public override void OnHurt (Entity offender)
	{
		//if hit unaware, stagger then aggro
		if(_AIState != EnemyState.Chase)
		{
			sight.target = offender;
			FoundTarget(offender);
		}

		//reset search time each time hit
		t = searchTime;
		//face the attacker on each hit
		Facing = IsTargetLeft() ? Facing.left : Facing.right;
	}

	public override IEnumerator Patrol ()
	{
		return base.Patrol();
	}

	public override void OnDeath()
	{
		base.OnDeath();
		Destroy(this.gameObject);
	}

	public override IEnumerator Chase ()
	{
		while(_AIState == EnemyState.Chase)
		{
			if(canFace) Facing = IsTargetLeft() ? Facing.left : Facing.right;
			if(canMove) normalizedHorizontalSpeed = IsTargetLeft() ? -1 : 1;
			else normalizedHorizontalSpeed = 0;

			//attack if can
			if(!attackState.inState 
			&& !specialAttackState.inState
			&& !slideAttackState.inState)
			{
				//regular distance attack
				if(sight.Distance <= tooCloseRange)
				{
					_anim.SetTrigger("StrongAttack");
				}

				//too close distance attack
				else if(sight.Distance <= attackRange)
				{
					Attack();
				}
			}

			yield return null;
		}
	}

	public override IEnumerator Search ()
	{
		return base.Search ();
	}
}
