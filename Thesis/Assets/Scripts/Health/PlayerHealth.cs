using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : Health {

	Evolution _evo;
	HeartBeat heartbeat;

	public WeaponController _switcher;
	public AnimationMethods animMethods;
	public Player player;

	public override void Start () {
		_evo = LevelManager.Instance.GetComponent<Evolution>();
		_healthBar = GameObject.Find("UI").GetComponent<UI>().MyHealthBar;
		heartbeat = GameObject.Find("PlayerBars").transform.Find("HealthBar/Icon").GetComponent<HeartBeat>();

		//set health to saved or max depending on if game exists
		health = (PlayerPrefs.GetInt("GameCreated") == 1) ? PlayerPrefs.GetInt("Health") : maxHealth;
		UpdateHealthBar();
	}

	public override void TakeDamage(Entity e, int dmg)
	{
		if(animMethods.invincible) return;
		if(player.blockState.inState && !_switcher.IsWeapon(Weapons.Bow)) dmg = (int)Mathf.Ceil(dmg/2);

		base.TakeDamage(e, dmg);
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
		if(health == maxHealth) return;
		_evo.SubtractBlood(1);
		base.Heal();
	}

	public override void Heal(int heal)
	{
		if(_evo.Blood == 0) return;
		if(health == maxHealth) return;
		_evo.SubtractBlood(1);
		base.Heal(heal);
	}

	public override void OnKill()
	{
		//reset health to max
		health = maxHealth;
		PlayerPrefs.SetInt("Health", maxHealth);
		PlayerPrefs.Save();

		base.OnKill();
	}
}
