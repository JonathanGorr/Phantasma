using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonShieldAI : EnemyAI {

	public float blockTime = 2;
	[SerializeField] float blockT;
	public ParticleSystem sparks;

	public override void Awake()
	{
		blockT = blockTime;
		base.Awake();
	}

	public override void FoundTarget(Entity e)
	{
		base.FoundTarget(e);
	}

	public override void BackStep()
	{
		//TODO: enabling this causes the enemy to shoot up in the air for some strange unknown reason
		// must have something to do with force, it creates crazy deltaMovement.y values
		//base.BackStep();
	}

	public override void LostTarget(Entity e)
	{
		base.LostTarget(e);
	}

	void BlockEnter()
	{
		SetDamping(actionDamping);
		canMove = false;
	}
	void BlockExit()
	{
		SetDamping(groundDamping);
		canMove = true;
	}

	public override void Block()
	{
		//don't block if already attacking or blocking
		if(specialAttackState.inState) return;
		if(attackState.inState) return;
		if(blockState.inState) return;

		StartCoroutine(BlockRoutine());
	}

	public override void OnHurt (Entity offender)
	{
		if(blockState.inState)
		{
			SlideBlocking();
			sparks.Emit(Random.Range(10, 15));
			SFX.Instance.PlayFX("block", myTransform.position);
			blockT = blockTime;
		}
		else
		{
			Block();
			SFX.Instance.PlayFX("hurt_skeleton", myTransform.position);
		}
		base.OnHurt (offender);
	}

	IEnumerator BlockRoutine()
	{
		combatState = CombatState.Blocking;
		_anim.SetBool("Blocking", true);
		canMove = false;
		normalizedHorizontalSpeed = 0;

		while(blockT > 0)
		{
			blockT -= Time.deltaTime;
			yield return null;
		}
		blockT = blockTime;

		canMove = true;
		_anim.SetBool("Blocking", false);
	}

	public override void OnAttackEnter()
	{
		SFX.Instance.PlayFX("windup_heavy", myTransform.position);
		base.OnAttackEnter();
	}

	public override void OnSpecialAttackEnter()
	{
		SFX.Instance.PlayFX("windup_heavy", myTransform.position);
		base.OnSpecialAttackEnter();
	}

	public override void OnDeath()
	{
		SFX.Instance.PlayFX("death_skeleton", myTransform.position);
		base.OnDeath();
	}
}
