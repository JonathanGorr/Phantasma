using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Health : MonoBehaviour {

	[Header("Health")]
	public int health;
	public int maxHealth;

	[Header("Bools")]
	public bool	dead;
	public bool	aggro;
	public bool	invincible;

	private Explode _explode;
	public Entity entity;
	public GameObject healthBarPrefab;

	Slider _healthBar;
	Evolution _evo;
	WeaponController _switcher;
	HitPoints _hitPoints;
	PlayerPreferences _prefs;
	MusicFader _mFader;
	PlayerInput _input;
	SFX _sfx;
	Follow healthBarFollow;

	public Vector2 healthBarOffset = new Vector2(0,1);

	void Start ()
	{
		if(gameObject.CompareTag("Player")) 
		{
			_evo = GameObject.Find ("_LevelManager").GetComponent<Evolution>();
			_healthBar = GameObject.Find("UI").GetComponent<UI>().MyHealthBar;
		}
		else
		{
			GameObject bar = Instantiate(healthBarPrefab);
			healthBarFollow = bar.GetComponent<Follow>();
			healthBarFollow.Target = this.transform;
			healthBarFollow.Offset = healthBarOffset;
			_healthBar = bar.GetComponentInChildren<Slider>();
		}

		health = maxHealth;
		UpdateHealthBar();
		//import the enemy script
		_input = GameObject.Find ("_LevelManager").GetComponent<PlayerInput> ();
		_hitPoints = GetComponent<HitPoints>();
		_explode = GetComponent<Explode>();
		_mFader = GameObject.Find ("Music").GetComponent<MusicFader>();
		_sfx = _input.GetComponent<SFX>();
	}

	void FixedUpdate()
	{
		//clamp health
		Mathf.Clamp (health, 0, maxHealth);
	}

	public void TakeDamage(int dmg)
	{
		if(invincible) return;
		entity.OnHurt();
		//if blocking, damage is half and you dont get knocked back
		if(entity.combatState == CombatState.Blocking)
		{
			entity._stamina.UseStamina(entity.blockedHitDrain);
			_sfx.PlayFX("block", transform.position);
			entity.Slide();
			dmg = (int)Mathf.Ceil(dmg/2);
		}

		_hitPoints.TakeDamage(dmg);
		//when a weapon collides, subtract health by the passes int(damage)
		health -= dmg;
		//if an enemy has no health left, drop blood and destroy object
		if (health <= 0)
		{
			dead = true;
			OnKill();
		}
		UpdateHealthBar();
	}

	public void Heal()
	{
		if(health == maxHealth || health == 0) return;
		if(_evo.blood == 0) return;

		_evo.SubtractBlood(1);
		health += 1;
		_hitPoints.Heal(1);
		_sfx.PlayFX("heal", transform.position);
		UpdateHealthBar();
	}

	public void Heal(int heal)
	{
		if(heal <= 0 || health == maxHealth || health == 0) return;
		if(_evo.blood == 0) return;

		_evo.SubtractBlood(1);
		health += heal;
		_hitPoints.Heal(heal);
		_sfx.PlayFX("heal", transform.position);
		UpdateHealthBar();
	}

	public void UpdateHealthBar()
	{
		_healthBar.maxValue = maxHealth;
		_healthBar.value = health;
	}

	public void SetInvincible(bool truth)
	{
		invincible = truth;
	}

	public void OnKill()
	{
		entity.OnDeath();
		healthBarFollow.OnDeath();
		if(gameObject.name == "Boss")
		{
			_mFader.FadeTo(_mFader.victoryTheme);
		}

		if(_explode)
		{
			//enemyHurt = false;
			_explode.OnExplode();
		}
		else
			print("enemy was already destroyed");
	}
}
