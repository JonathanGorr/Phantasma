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

	public override void OnHurt ()
	{
		base.OnHurt ();
	}

	public override IEnumerator Patrol ()
	{
		debugColor = Color.green;
		while(_AIState == EnemyState.Patrol)
		{
			target = CircleCast();

			if(target)
			{
				target._cam.RegisterMe(myTransform);
				_AIState = EnemyState.Chase;
			}

			yield return null;
		}
	}

	public override IEnumerator Chase ()
	{
		debugColor = Color.red;
		while(_AIState == EnemyState.Chase)
		{
			if (Distance() < attackRange)
			{
				canAttack = false;
				speed = 0;

				//choose random attack min max, result != max
				int randomAttack = Random.Range(1, 3);
				//choose 1 of 2 attacks that are close range
				int closeAttack = Random.Range (2, 4);

				_anim.SetTrigger("" + randomAttack);
				//_weapon.Attack(0);

				yield return new WaitForSeconds (attackDelay);
				canAttack = true;
			}
			yield return null;
		}
	}

	public override IEnumerator Return ()
	{
		return base.Return ();
	}
}
