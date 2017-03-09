using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : Health {

	Evolution _evo;
	WeaponController _switcher;
	HeartBeat heartbeat;

	public override void Start () {
		_evo = LevelManager.Instance.GetComponent<Evolution>();
		_healthBar = GameObject.Find("UI").GetComponent<UI>().MyHealthBar;
		heartbeat = GameObject.Find("PlayerBars").transform.Find("HealthBar/Icon").GetComponent<HeartBeat>();
	}

	public override void UpdateHealthBar()
	{
		base.UpdateHealthBar();
		//used to inform the heart icon animator of how fast to beat the heart- based on the ratio of health to max health
		heartbeat.Set(1-Mathf.Abs((float)health/(float)maxHealth));
	}

	public override void Heal()
	{
		if(_evo.Blood == 0) return;
		_evo.SubtractBlood(1);
		base.Heal();
	}

	public override void Heal(int heal)
	{
		if(_evo.Blood == 0) return;
		_evo.SubtractBlood(1);
		base.Heal(heal);
	}

	public override void OnKill()
	{
		base.OnKill();
	}
}
