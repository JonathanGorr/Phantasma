using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Health : MonoBehaviour {

	bool killed = false;

	[Header("Health")]
	public int health;
	public int maxHealth;

	[Header("Bools")]
	public bool	aggro;
	public bool	invincible;
	public bool blocking;

	public bool Dead
	{
		get { return health <= 0; }
	}

	[Header("Refs")]
	public HitPoints _hitPoints;

	public Slider _healthBar;

	public delegate void OnHurt(Entity e);
	public event OnHurt onHurt;

	public delegate void OnDeath();
	public event OnDeath onDeath;

	public float HealthRatio
	{
		get { return health/maxHealth; }
	}

	public virtual void Start ()
	{
		health = maxHealth;
		UpdateHealthBar();
	}

	public virtual void TakeDamage(Entity e, int dmg)
	{
		if(killed) return;
		if(invincible) return;

		if(blocking) dmg = (int)Mathf.Ceil(dmg/2);
		if(_hitPoints) _hitPoints.TakeDamage(dmg);
		//when a weapon collides, subtract health by the passes int(damage)
		health = Mathf.Clamp (health - dmg, 0, maxHealth);
		//if an enemy has no health left, drop blood and destroy object
		if (Dead) { OnKill(); }
		else { if(onHurt != null) onHurt(e); }
		UpdateHealthBar();
	}

	public virtual void Heal()
	{
		if(killed) return;

		if(health == maxHealth || health == 0) return;
		health += 1;
		_hitPoints.Heal(1);
		SFX.Instance.PlayFX("heal", transform.position);
		UpdateHealthBar();
	}

	public virtual void Heal(int heal)
	{
		if(killed) return;

		if(heal <= 0 || health == maxHealth || health == 0) return;
		health += heal;
		_hitPoints.Heal(heal);
		SFX.Instance.PlayFX("heal", transform.position);
		UpdateHealthBar();
	}

	public virtual void UpdateHealthBar()
	{
		_healthBar.maxValue = maxHealth;
		_healthBar.value = health;
	}

	public void SetInvincible(bool truth)
	{
		invincible = truth;
	}

	public virtual void OnKill()
	{
		if(killed) return;
		killed = true;
		if(onDeath!=null) onDeath();
	}
}
