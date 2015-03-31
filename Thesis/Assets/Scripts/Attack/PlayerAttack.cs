using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour {
	
	//import animator via reference
	private Animator _animator;
	//import Player script
	private Player _player;
	private WeaponSFX _wepSFX;
	private Health _health;
	private CharacterController2D _controller;
	public bool readyToAttack;
	private bool _attack;
	private bool _strongAttack;
	private bool blocking;

	//damages
	private int damage;
	public int r1Damage;
	public int r2Damage;
	public int blockingAttackDmg;

	public GameObject hitSprite;

	// Use this for initialization
	void Awake () {
		//get the animator component
		_animator = GetComponent<Animator>();
		_wepSFX = GetComponent<WeaponSFX>();
		_player = GetComponentInParent<Player>();
		_controller = GetComponentInParent<CharacterController2D>();
	}

	void Update()
	{
		blocking = Input.GetMouseButton (1) || Input.GetButton ("360_LeftBumper");
		_attack = Input.GetMouseButtonDown (0) || Input.GetButtonDown("360_RightBumper");
		_strongAttack = Input.GetKeyDown(KeyCode.E) || Input.GetAxis("360_Triggers")>0.3;
	}

	void FixedUpdate()
	{
		if(_player != null)
		{
			//regular attack
			if((_attack) && (!blocking) && _player.moving == false)
			{
				r1Attack();
				_animator.SetTrigger("Attack");
			}
			//blocking attack
			else if((blocking) && (_attack))
			{
				BlockingAttack();
				_animator.SetTrigger("BlockingAttack");
			}
			//overheadstrike
			else if(_strongAttack && _controller.isGrounded && !blocking)
			{
				r2Attack();
				_animator.SetTrigger("StrongAttack");
			}
		}
	}

	void Ready()
	{
		readyToAttack = true;
	}

	void NotReady()
	{
		readyToAttack = false;
	}

	public void r1Attack()
	{
		damage = r1Damage;
	}

	public void r2Attack()
	{
		damage = r2Damage;
	}

	public void BlockingAttack()
	{
		damage = blockingAttackDmg;
	}

	void OnTriggerStay2D(Collider2D target)
	{
		if (target.gameObject.tag == "Enemy")
		{
			//test if the player is ready to attack
			if(readyToAttack)
			{
				var _health = target.GetComponent<Health>();
				//instantiate hit sprite at collider
				Transform source = GetComponent<BoxCollider2D>().transform;
				_health.EnemyTakeDamage(damage);
				//instantiate hit sprite
				Instantiate(hitSprite, source.position, source.rotation);
				readyToAttack = false;
			}
		}
		else if (target.gameObject.tag == "Shield")
		{
			//test if the player is ready to attack
			if(readyToAttack)
			{
				if(_wepSFX != null)
					_wepSFX.ShieldCollideSound();
				readyToAttack = false;
			}
		}
		else if (target.gameObject.tag == "Weapon")
		{
			//test if the player is ready to attack
			if(readyToAttack)
			{
				if(_wepSFX != null)
					_wepSFX.WeaponCollideSound();
				readyToAttack = false;
			}
		}
	}

	//when the enemy is already dealt damage, dont keep dealing; once per swing
	void onTriggerExit2D (Collider2D target)
	{
		readyToAttack = false;
	}
}
