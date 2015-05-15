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
	public bool readyToAttack = true;
	public float radius = .3f;
	private Transform _tip;
	private Vector2 location;

	//damages
	private int damage;
	public int r1Damage;
	public int r2Damage;
	public int blockingAttackDmg;
	private float delay = .3f;

	public GameObject hitSprite;

	void Awake () {
		_input = GameObject.Find ("_LevelManager").GetComponent<PlayerInput> ();
		_animator = GetComponent<Animator>();
		_player = GetComponentInParent<Player>();
		_controller = GetComponentInParent<CharacterController2D>();
		_tip = transform.Find ("Tip");
	}

	void Update()
	{
		if(_tip) location = new Vector2 (_tip.position.x, _tip.position.y);

		//arrow attack
		if(gameObject.tag == "Arrow")
		{
			delay = 1f;
			r1Attack();
			StartCoroutine("Damage");
			_animator.SetTrigger("Attack");
		}

		if(_player)
		{
			if(readyToAttack)
			{
				if(_player.normalizedHorizontalSpeed < 0.3f)
				{
					//regular attack
					if(_input._attack && !_input._blocking)
					{
						delay = .3f;
						r1Attack();
						StartCoroutine("Damage");
						_animator.SetTrigger("Attack");
					}

					//blocking attack
					else if(_input._blocking && _input._attack)
					{
						delay = .3f;
						BlockingAttack();
						StartCoroutine("Damage");
						_animator.SetTrigger("BlockingAttack");
					}

					//Strong Attack
					else if(_input._strongAttack && _controller.isGrounded && !_input._blocking)
					{
						delay = .05f;
						r2Attack();
						StartCoroutine("Damage");
						_animator.SetTrigger("StrongAttack");
					}
				}
			}
		}
	}

	IEnumerator Damage()
	{
		Collider2D[] enemies = Physics2D.OverlapCircleAll(location, radius);
		
		if(enemies.Length > 0)
		{
			foreach(Collider2D enemy in enemies)
			{
				if(enemy.gameObject)
				{
					if(enemy.gameObject.tag == "Enemy")
					{
						if(readyToAttack)
						{
							Health _health = enemy.GetComponent<Health>();
							//instantiate hit sprite at collider
							Transform source = GetComponent<BoxCollider2D>().transform;
							_health.EnemyTakeDamage(damage);
							readyToAttack = false;
							//instantiate hit sprite
							Instantiate(hitSprite, source.position, source.rotation);
							yield return new WaitForSeconds(delay);
							readyToAttack = true;
						}
					}
				}
			}
		}
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

	//when the enemy is already dealt damage, dont keep dealing; once per swing
	void onTriggerExit2D (Collider2D target)
	{
		readyToAttack = true;
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawSphere (location, radius);
	}
}
