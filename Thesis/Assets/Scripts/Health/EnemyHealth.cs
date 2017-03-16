using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : Health {

	public Vector2 healthBarOffset = new Vector2(0,1);
	public GameObject healthBarPrefab;
	Follow healthBarFollow;

	public AnimationMethods animMethods;
	public Enemy myEnemy;

	public override void Start()
	{
		GameObject bar = Instantiate(healthBarPrefab);
		healthBarFollow = bar.GetComponent<Follow>();
		healthBarFollow.Target = this.transform;
		healthBarFollow.Offset = healthBarOffset;
		_healthBar = bar.GetComponentInChildren<Slider>();

		base.Start();
	}

	public override void TakeDamage(Entity e, int dmg)
	{
		if(animMethods.invincible) return;
		if(myEnemy.blockState)
		{
			if(myEnemy.blockState.inState) 
			{
				dmg = 0;
				//dmg = (int)Mathf.Ceil(dmg/2); //TODO: replace with shield damage reduction
			}
		}

		base.TakeDamage(e, dmg);
	}

	public override void OnKill()
	{
		base.OnKill();
		this.healthBarFollow.OnDeath();
	}
}
