using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : EnemyAI {

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
		return base.Patrol();
	}

	public override void OnDeath()
	{
		base.OnDeath();
		Destroy(this.gameObject);
	}

	public override IEnumerator Chase ()
	{
		debugColor = Color.red;

		while(_AIState == EnemyState.Chase)
		{
			if (Distance() < attackRange)
			{
				speed = 0;

				//choose random attack min max, result != max
				//int randomAttack = Random.Range(1, 3);
				//choose 1 of 2 attacks that are close range
				int closeAttack = Random.Range (2, 4);

				if(Distance() < tooCloseRange)
				{
					_anim.SetTrigger("" + closeAttack);
				}
				else
				{
					//else random if more than one anim exists
					_anim.SetTrigger("1");
				}

				if(_health.health < 50)
					attackDelay = .1f;

				yield return new WaitForSeconds (attackDelay);
			}
			yield return null;
		}
	}

	public override IEnumerator Search ()
	{
		return base.Search ();
	}
}
