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

	[HideInInspector]
	public int playerDamage, enemyDamage;

	private Explode _explode;

	public Entity entity;
	private WeaponController _switcher;
	private HitPoints _hitPoints;
	private PlayerPreferences _prefs;
	private MusicFader _mFader;
	private PlayerInput _input;

	void Start () 
	{
		health = maxHealth;
		//import the enemy script
		_input = GameObject.Find ("_LevelManager").GetComponent<PlayerInput> ();
		_switcher = GameObject.Find("_LevelManager").GetComponent<WeaponController>();
		_hitPoints = GetComponent<HitPoints>();
		_explode = GetComponent<Explode>();
		_mFader = GameObject.Find ("Music").GetComponent<MusicFader>();
	}

	void FixedUpdate()
	{
		//clamp health
		Mathf.Clamp (health, 0, maxHealth);
	}

	public void TakeDamage(int value)
	{
		if(invincible) return;

		entity.OnHurt();
				
		//if blocking, damage is half and you dont get knocked back
		if(_input._blocking && _switcher.IsWeapon(Weapons.SwordShield))
		{
			playerDamage = value/2;
			print ("half damage:" + "only" + playerDamage + " of " + value);
			_hitPoints.TakeDamage(playerDamage);
		}
		else
		{
			playerDamage = value;
			_hitPoints.TakeDamage(playerDamage);
		}

		//when a weapon collides, subtract health by the passes int(damage)
		health -= playerDamage;

		//if an enemy has no health left, drop blood and destroy object
		if (health <= 0)
		{
			dead = true;
			OnKill();
		}
	}

	public void Heal(int heal)
	{
		if(heal > 0)
		{
			health += heal;
			_hitPoints.Heal(heal);
		}
	}

	public void SetInvincible(bool truth)
	{
		invincible = truth;
	}

	public void OnKill()
	{
		entity.OnDeath();

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
