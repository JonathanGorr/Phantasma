using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : EnemyAI {

	//weps
	private EnemyAttack _bossWeapon1;
	private EnemyAttack _bossWeapon2;

	public override void Awake ()
	{
		//boss weapon animators
		_bossWeapon1 = GameObject.Find("BossSwordLeft").GetComponent<EnemyAttack>();
		_bossWeapon2 = GameObject.Find("BossSwordRight").GetComponent<EnemyAttack>();
		if(!_bossWeapon1 || !_bossWeapon2) print("Boss Weapon not found");

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

				if(Distance() < tooCloseRange)
				{
					_anim.SetTrigger("" + closeAttack);
					_bossWeapon1.Attack(closeAttack);
					_bossWeapon2.Attack(closeAttack);
				}
				else
				{
					//else random if more than one anim exists
					_anim.SetTrigger("1");
					_bossWeapon1.Attack(1);
					_bossWeapon2.Attack(1);
				}

				if(_health.health < 50)
					attackDelay = .1f;

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
