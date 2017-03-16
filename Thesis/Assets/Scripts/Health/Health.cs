using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Health:
/// The amount of vitality attributed to an entity.
/// Things that typically have 'health':
/// enemies, player, breakable objects
/// </summary>

public class Health : MonoBehaviour {

	[Header("Health")]
	public int health;
	public int maxHealth;

	[Header("Bools")]
	public bool	aggro;
	public bool	invincible;

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

	public virtual void OnKill()
	{
		OnDeath died = onDeath;
		if(died != null) died();
	}

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
		if(Dead) return;
		if(invincible) return;

		if(dmg > 0)
		{
			if(_hitPoints) _hitPoints.TakeDamage(dmg);
			//when a weapon collides, subtract health by the passes int(damage)
			health = Mathf.Clamp (health - dmg, 0, maxHealth);
		}
		//if an enemy has no health left, drop blood and destroy object
		if (Dead) { OnKill(); }
		else { if(onHurt != null) onHurt(e); }
		UpdateHealthBar();
	}

	public virtual void Heal()
	{
		if(Dead) return;

		if(health == maxHealth || health == 0) return;
		health += 1;
		_hitPoints.Heal(1);
		SFX.Instance.PlayFX("heal", transform.position);
		UpdateHealthBar();
	}

	public virtual void Heal(int heal)
	{
		if(Dead) return;

		if(heal <= 0 || health == maxHealth || health == 0) return;
		health += heal;
		_hitPoints.Heal(heal);
		SFX.Instance.PlayFX("heal", transform.position);
		UpdateHealthBar();
	}

	public virtual void UpdateHealthBar()
	{
		if(!_healthBar) return;
		_healthBar.maxValue = maxHealth;
		_healthBar.value = health;
	}

	public void SetInvincible(bool truth)
	{
		invincible = truth;
	}
}
