using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonSmallAI : EnemyAI {

	public override void Awake ()
	{
		base.Awake ();
		animMethods.onCollapse += CollapseParticles;
	}

	public override void Move ()
	{
		base.Move ();
	}

	public override void OnHurt (Entity offender)
	{
		SFX.Instance.PlayFX("hurt_skeleton", myTransform.position);
		base.OnHurt (offender);
	}

	public override IEnumerator Patrol ()
	{
		return base.Patrol();
	}

	public override IEnumerator Chase ()
	{
		return base.Chase();
	}

	public override IEnumerator Search ()
	{
		return base.Search ();
	}

	void CollapseParticles()
	{
		_footDust.Emit(10);
	}

	public override void OnDeath()
	{
		SFX.Instance.PlayFX("death_skeleton", myTransform.position);
		base.OnDeath();
	}
}
