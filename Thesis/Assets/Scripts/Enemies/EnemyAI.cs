using UnityEngine;
using System.Collections;

public enum EnemyState { Patrol, Chase, Return }
public class EnemyAI : Enemy
{
	[Header("Enemy AI")]
	public bool attacking;
	public EnemyState _AIState;
	public LayerMask layers;
	private Vector2 _velocity;
	[HideInInspector] public Player target;

	public float lookAtDistance = 10f;
	public float chaseRange = 7f;
	public float attackRange = 2f;
	public float sightRadius = 1f;
	public float knockBackForce = 220f;
	public float tooCloseRange = 1f;

	[Header("Parms")]
	public float jumpForce = 17f;
	public float attackDelay = 0.5f;

	[Header("Local References")]
	public SpriteRenderer rend;
	public Rigidbody2D rbody;
	public EnemyAttack _weapon;
	public Health _health;

	[HideInInspector] public bool canAttack = true;
	[HideInInspector] public Vector3 moveDirection = Vector3.zero;
	[HideInInspector] public bool left;
	[HideInInspector] public Vector2 faceLeft;
	[HideInInspector] public Vector2 faceRight;

	[HideInInspector] public Color debugColor;
	
	public override void Awake()
	{
		base.Awake();
		faceLeft = new Vector2(myTransform.localScale.x, myTransform.localScale.y);
		faceRight = new Vector2(-myTransform.localScale.x, myTransform.localScale.y);
		StartCoroutine(StateMachine());
		StartCoroutine(LookAt());
	}

	public virtual IEnumerator StateMachine()
	{
		while(true) 
		{
			//don't update if invisible
			if(!rend.isVisible) yield return null;
			yield return StartCoroutine(_AIState.ToString());
		}
	}

	public float Distance() //returns the distance between me and my target
	{
		return Vector2.Distance(myTransform.position, target.myTransform.position);
	}

	public Player CircleCast() //casts a circle to detect targets
	{
		Collider2D col = Physics2D.OverlapCircle(transform.position, sightRadius, layers);
		if(col != null) return col.GetComponent<Player>();
		else return null;
	}

	public virtual IEnumerator Patrol()
	{
		debugColor = Color.green;
		while(_AIState == EnemyState.Patrol)
		{
			yield return null;
		}
	}

	public virtual IEnumerator Chase()
	{
		debugColor = Color.red;
		while(_AIState == EnemyState.Chase)
		{
			yield return null;
		}
	}

	public virtual IEnumerator Return()
	{
		debugColor = Color.yellow;
		while(_AIState == EnemyState.Return)
		{
			yield return null;
		}
	}

	public override void OnDeath()
	{
		if(target) target._cam.UnRegisterMe(myTransform);
	}

	public override void OnHurt()
	{
		//knockback
		if(left)
		{
			rbody.AddForce(Vector3.right * knockBackForce);
		}
		else if(!left)
		{
			rbody.AddForce(-Vector3.right * knockBackForce);
		}
	}

	public virtual void Move()
	{
		if(canAttack)
		{
			//player is on the right side
			if(target.myTransform.position.x < myTransform.position.x)
			{
				left = true;
			}
			//if the player is left
			else if(target.myTransform.position.x > myTransform.position.x)
			{
				left = false;
			}
			//if distance is within lookatdistance, run the method
			if(Distance() < lookAtDistance)
			{
				LookAt();
				speed = 0;
			}
			//if player is not within the lookatdistance, show green
			if(Distance() > lookAtDistance)
			{
				speed = 0;
				//color = green;
			}
			//if player is within chase distance, chase
			else if (Distance() < chaseRange && _health.aggro == false)
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

		//foot dust
		if(Mathf.Abs(_velocity.x) < 0.1f)
			_footDust.Play();

		//set speed for transitioning movement
		_anim.SetFloat ("Speed", speed);
	}
	
	void FixedUpdate()
	{
		if(!rend.isVisible) return;
		if(!target) return;

		Move();
	}
	
	IEnumerator LookAt()
	{
		while(true)
		{
			while(target == null) yield return null;

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

			yield return null;
		}
	}
	
	void ChasePlayer(float modifiedSpeed)
	{
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
		_controller.move(moveDirection * speed * Time.deltaTime);
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

	public IEnumerator Jump()
	{
		//TODO: make the jump the same as the players, not floaty like this one...
		GetComponent<Rigidbody2D>().AddForce(Vector3.up * jumpForce);
		yield return new WaitForSeconds (1f);
	}

	void OnCollisionStay2D(Collision2D obstacle)
	{
		//dont let enemies collide with eachother
		if(obstacle.gameObject.tag != "Enemy" && obstacle.gameObject.tag != "Blood")
			StartCoroutine(Jump());
	}

	void OnDrawGizmos()
	{
		Gizmos.color = debugColor;
		Gizmos.DrawWireSphere(transform.position, sightRadius);
	}
}