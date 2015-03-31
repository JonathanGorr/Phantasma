using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Health : MonoBehaviour {

	public int 
		health,
		maxHealth;

	public bool 
		playerHurt,
		enemyHurt,
		healing, 
		dead,
		aggro,
		invincible;

	[HideInInspector]
	public int playerDamage, enemyDamage;

	private Explode _explode;

	private WeaponSwitcher _switcher;
	private HitPoints _hitPoints;
	private PlayerPreferences _prefs;

	// Use this for initialization
	void Awake () {
		health = maxHealth;
		//import the enemy script
		_switcher = GameObject.Find("_LevelManager").GetComponent<WeaponSwitcher>();
		_hitPoints = GetComponent<HitPoints>();
		_explode = GetComponent<Explode>();
	}

	void FixedUpdate()
	{
		healing = false;
		//clamp health
		Mathf.Clamp (health, 0, maxHealth);
	}

	public void PlayerTakeDamage(int value)
	{
		if(!invincible)
		{
			//entity is hurt(player), trigger a knockkback animation
			playerHurt = true;

			//if blocking, damage is half and you dont get knocked back
			if(Input.GetButton("360_LeftBumper") && _switcher.currentWeapon == 1)
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
		}

		//if an enemy has no health left, drop blood and destroy object
		if (health <= 0)
		{
			dead = true;
			OnKill();
		}
	}

	public void EnemyTakeDamage(int value)
	{
		//when a weapon collides, subtract health by the passes int(value)
		health -= value;
		//entity is hurt(player), trigger a knockkback animation
		enemyHurt = true;
		//assign the hitpoint to the damage done
		enemyDamage = value;
		//instantiate hitpoint
		_hitPoints.TakeDamage(enemyDamage);
		//make enemy aware of player and chase if damaged
		aggro = true;

		//if an enemy has no health left, drop blood and destroy object
		if (health <= 0)
		{
			OnKill();
		}
	}

	public void Heal(int heal)
	{
		health += heal;
		healing = true;
		if(_hitPoints != null)
			_hitPoints.Heal(heal);
	}

	public void Invincible()
	{
		invincible = !invincible;
	}

	public void OnKill()
	{
		if(_explode != null)
		{
			//enemyHurt = false;
			_explode.OnExplode();

			if(gameObject.tag == "Enemy")
			{
				enemyHurt = false;
				Destroy(gameObject);
			}
			else if(gameObject.tag == "Player")
			{
				playerHurt = false;
			}
		}
		else
			print("enemy was already destroyed");
	}
}
