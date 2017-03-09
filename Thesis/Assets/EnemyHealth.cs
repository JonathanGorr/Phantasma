using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : Health {

	public Vector2 healthBarOffset = new Vector2(0,1);
	public GameObject healthBarPrefab;
	Follow healthBarFollow;

	public override void Start()
	{
		GameObject bar = Instantiate(healthBarPrefab);
		healthBarFollow = bar.GetComponent<Follow>();
		healthBarFollow.Target = this.transform;
		healthBarFollow.Offset = healthBarOffset;
		_healthBar = bar.GetComponentInChildren<Slider>();

		base.Start();
	}

	public override void OnKill()
	{
		base.OnKill();
		this.healthBarFollow.OnDeath();
	}
}
