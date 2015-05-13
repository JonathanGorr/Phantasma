using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour {
	
	//import animator via reference
	private Animator _animator;
	//import Player script
	private Player _player;
	private Health _health;
	private PlayerInput _input;
	private CharacterController2D _controller;
	private LevelManager _manager;
	public bool readyToAttack;
	private MusicFader _mFader;

	//damages
	private int damage;
	public int r1Damage;
	public int r2Damage;
	public int blockingAttackDmg;
	public float delay = .2f;

	public GameObject hitSprite;

	void Awake () {
		_manager = GameObject.Find ("_LevelManager").GetComponent<LevelManager> ();
		_mFader = GameObject.Find("Music").GetComponent<MusicFader> ();
		_input = GameObject.Find ("_LevelManager").GetComponent<PlayerInput> ();
		_animator = GetComponent<Animator>();
		_player = GetComponentInParent<Player>();
		_controller = GetComponentInParent<CharacterController2D>();
	}

	void Update()
	{
		if(_player)
		{
			//regular attack
			if((_input._attack) && (!_input._blocking) && !_player.moving)
			{
				r1Attack();
				_animator.SetTrigger("Attack");
			}
			//blocking attack
			else if((_input._blocking) && (_input._attack))
			{
				BlockingAttack();
				_animator.SetTrigger("BlockingAttack");
			}
			//Strong Attack
			else if(_input._strongAttack && _controller.isGrounded && !_input._blocking)
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
			if(_manager.canTransition)
			{
				_mFader.Fade(_mFader.battleTheme);
				_manager.canTransition = false;
			}
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
		/*
		else if (target.gameObject.tag == "Shield")
		{
			//test if the player is ready to attack
			if(readyToAttack)
			{
				if(_wepSFX) _wepSFX.ShieldCollideSound();
				readyToAttack = false;
			}
		}
		else if (target.gameObject.tag == "Weapon")
		{
			//test if the player is ready to attack
			if(readyToAttack)
			{
				if(_wepSFX) _wepSFX.WeaponCollideSound();
				readyToAttack = false;
			}
		}
		*/
	}

	//when the enemy is already dealt damage, dont keep dealing; once per swing
	void onTriggerExit2D (Collider2D target)
	{
		//readyToAttack = false;
	}
}
