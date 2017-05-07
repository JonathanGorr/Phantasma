using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThiefAI : EnemyAI {

	public override void Awake ()
	{
		base.Awake ();
	}

	public override void Move ()
	{
		base.Move ();
	}

	public override void OnHurt (Entity offender)
	{
		base.OnHurt (offender);
	}

	public override IEnumerator Patrol ()
	{
		while(_AIState == EnemyState.Patrol)
		{
			if(sight.CanPlayerBeSeen())
			{
				_AIState = EnemyState.Chase;
			}

			yield return null;
		}
	}

	public override IEnumerator Chase ()
	{
		while(_AIState == EnemyState.Chase)
		{
			if (sight.Distance < attackRange)
			{
				speed = 0;
				Attack();
			}
			yield return null;
		}
	}

	public override IEnumerator Search ()
	{
		return base.Search ();
	}
}
