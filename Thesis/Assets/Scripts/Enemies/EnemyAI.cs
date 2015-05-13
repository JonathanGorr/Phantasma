using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
	private float distance;

	private Vector2 _velocity;

	public float
	lookAtDistance,
	chaseRange,
	attackRange,
	tooCloseRange = 1f,
	gravity,
	knockBackDistance = 220f;
	
	private CharacterController2D _enemyController;
	private Animator _animator;
	private EnemyAttack _weapon;
	private Health _health;
	private ParticleSystem _footDust;
	//weps
	private GameObject _bossWeapon1;
	private GameObject _bossWeapon2;
	private bool ready = true;
	
	private Vector3 moveDirection = Vector3.zero;

	private float speed;
	public float walkSpeed = 2f;
	public float runSpeed = 4f;
	public float jumpForce = 17f;

	private bool left;
	public bool attacking;
	private Vector2 faceLeft;
	private Vector2 faceRight;

	private Transform _player;
	private Transform myTransform;

	//gizmos
	[SerializeField]
	[Range( 0, 5f )]
	public float size;
	//private Color color;
	public Color green;
	public Color yellow;
	public Color red;
	
	void Awake()
	{
		_enemyController = GetComponent<CharacterController2D> ();
		_animator = GetComponent<Animator> ();
		_weapon = GetComponentInChildren<EnemyAttack>();
		_player = GameObject.Find("_Player").transform;
		_health = GetComponent<Health> ();
		_footDust = GetComponentInChildren<ParticleSystem>();

		//boss weapon animators
		_bossWeapon1 = GameObject.Find("BossSwordLeft");
		_bossWeapon2 = GameObject.Find("BossSwordRight");

		if(_bossWeapon1 == null || _bossWeapon2 == null)
			print("weapons not found");
		//cache transform, dont call every frame 
		myTransform = transform;

		faceLeft = new Vector2(myTransform.localScale.x, myTransform.localScale.y);
		faceRight = new Vector2(-myTransform.localScale.x, myTransform.localScale.y);
	}
	
	void FixedUpdate()
	{
		//variable distance between the player and the enemy
		distance = Vector3.Distance(_player.position, myTransform.position);

		if(ready)
		{
			//player is on the right side
			if(_player.position.x < myTransform.position.x)
			{
				left = true;
			}
			//if the player is left
			else if(_player.position.x > myTransform.position.x)
			{
				left = false;
			}
			//if distance is within lookatdistance, run the method
			if(distance < lookAtDistance)
			{
				LookAt();
				speed = 0;
			}
			//if player is not within the lookatdistance, show green
			if(distance > lookAtDistance)
			{
				speed = 0;
				//color = green;
			}
			if (distance < attackRange)
			{
				StartCoroutine(AttackPlayer());
			}
			//if player is within chase distance, chase
			else if (distance < chaseRange && _health.aggro == false)
			{
				LookAt();
				ChasePlayer(walkSpeed);
			}
			else if(_health.aggro == true)
			{
				LookAt();
				ChasePlayer(runSpeed);
			}
		}

		if (_health.enemyHurt)
		{
			Knockback();
		}

		//foot dust
		if(Mathf.Abs(_velocity.x) < 0.1f)
			_footDust.Play();

		//set speed for transitioning movement
		_animator.SetFloat ("Speed", speed);
	}
	
	void LookAt()
	{
		//yellow color
		//color = yellow;

		if(!attacking)
		{
			//flip
			if(left)
			{
				myTransform.localScale = faceLeft;
			}
			else if(!left)
			{
				myTransform.localScale = faceRight;
			}
		}
		return;
	}
	
	void ChasePlayer(float modifiedSpeed)
	{
		//color = red;

		speed = modifiedSpeed;

		if(!attacking)
		{
			//player is on the right side
			if(left)
			{
				moveDirection = -myTransform.right;
			}
			//if the player is left
			else if(!left)
			{
				moveDirection = myTransform.right;
			}
		}
		//move enemy using the character controller 2d
		_enemyController.move(moveDirection * speed * Time.deltaTime);
		_velocity.x = Mathf.Lerp( _velocity.x, speed, Time.fixedDeltaTime);
	}

	public void Attacking()
	{
		attacking = true;
	}

	public void NotAttacking()
	{
		attacking = false;
	}

	private IEnumerator AttackPlayer()
	{
		ready = false;

		speed = 0;

		//choose random attack min max, result != max
		int randomAttack = Random.Range(1, 3);
		//choose 1 of 2 attacks that are close range
		int closeAttack = Random.Range (2, 4);

		//if the boss, give different instruction
		if(gameObject.name == "Boss")
		{
			if(distance < tooCloseRange)
			{
				_animator.SetTrigger("" + closeAttack);
				_bossWeapon1.GetComponent<EnemyAttack>().Attack(closeAttack);
				_bossWeapon2.GetComponent<EnemyAttack>().Attack(closeAttack);
			}
			else
			{
				//else random if more than one anim exists
				_animator.SetTrigger("1");
				_bossWeapon1.GetComponent<EnemyAttack>().Attack(1);
				_bossWeapon2.GetComponent<EnemyAttack>().Attack(1);
			}
		}
		//this is for non-boss enemies
		else
		{
			_animator.SetTrigger("" + randomAttack);
			_weapon.Attack(0);
		}

		yield return new WaitForSeconds (0.5f);
		ready = true;
	}

	public IEnumerator Jump()
	{
		//TODO: make the jump the same as the players, not floaty like this one...
		GetComponent<Rigidbody2D>().AddForce(Vector3.up * jumpForce);
		yield return new WaitForSeconds (0.5f);
	}

	public void Knockback()
	{
		if(left){
			GetComponent<Rigidbody2D>().AddForce(Vector3.right * knockBackDistance);
		}
		else if(!left)
		{
			GetComponent<Rigidbody2D>().AddForce(-Vector3.right * knockBackDistance);
		}

		_health.enemyHurt = false;
	}

	void OnCollisionStay2D(Collision2D obstacle)
	{
		//dont let enemies collide with eachother
		if(obstacle.gameObject.tag != "Enemy" && obstacle.gameObject.tag != "Blood")
			StartCoroutine(Jump());
	}
}