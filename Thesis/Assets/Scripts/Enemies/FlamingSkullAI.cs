using UnityEngine;
using System.Collections;

public class FlamingSkullAI : MonoBehaviour {
	
	public Transform _player;

	private Vector3 targetPosition;

	public float
		lookAtDistance,
		chaseRange,
		attackRange,
		distance,
		walkSpeed = 2f,
		runSpeed = 4f,
		attackSpeed = 6f,
		speed,
		attackRepeatTime = 4f,
		knockBackDistance = 220f;

	private Transform myTransform;
	private Animator _anim;
	
	public AudioClip _chatteringTeeth;
	public AudioClip _attack;

	private bool left;
	private bool attacking = false;
	private Vector2 faceLeft;
	private Vector2 faceRight;

	private Health _health;

	//gizmos
	[SerializeField]
	[Range( 0, 5f )]
	public float size;
	private Color color;
	public Color green;
	public Color yellow;
	public Color red;

	void Awake()
	{
		_anim = GetComponent<Animator> ();
		_player = GameObject.Find("_Player").transform;
		_health = GetComponent<Health> ();
		
		//cache transform, dont call every frame 
		myTransform = transform;

		//flipping
		faceLeft = new Vector2(transform.localScale.x, transform.localScale.y);
		faceRight = new Vector2(-transform.localScale.x, transform.localScale.y);

		targetPosition = _player.position;

		GetComponent<AudioSource>().clip = _chatteringTeeth;
		GetComponent<AudioSource>().Play ();
	}

	void FixedUpdate()
	{
		if(_player != null)
		{
			//variable distance between the player and the enemy
			distance = Vector2.Distance(_player.position, transform.position);

			//only update player position when chasing, not attacking
			if(!attacking)
			{
				//update target position
				targetPosition = _player.position;
			}

			//movement
			myTransform.position = Vector2.MoveTowards(myTransform.position, targetPosition, speed * Time.fixedDeltaTime);

			//player location
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
			if(distance < lookAtDistance && !attacking)
			{
				LookAt();
			}
			if(distance < chaseRange && !attacking)
			{
				LookAt();
				ChasePlayer(runSpeed);
			}

			//if player is not within the lookatdistance, show green
			if(distance > lookAtDistance && !attacking)
			{
				//play idle anim
				speed = 0;
				color = green;
			}

			//if player is within attack range, run the attack method
			if (distance < attackRange && Time.time > 0 && !attacking)
			{
				color = red;
				StartCoroutine(AttackPlayer());
			}

			if(_health.enemyHurt)
			{
				_health.enemyHurt = false;
				//Knockback();
			}
		}
	}

	void LookAt()
	{
		//yellow color
		color = yellow;
		
		//flip
		if(left)
		{
			myTransform.localScale = faceLeft;
		}
		else if(!left)
		{
			myTransform.localScale = faceRight;
		}
		return;
	}
	
	void ChasePlayer(float modifiedSpeed)
	{
		color = red;
		speed = runSpeed;
	}

	void AttackSound()
	{
		GetComponent<AudioSource>().PlayOneShot (_attack);
	}

	void Knockback()
	{
		if(left){
			GetComponent<Rigidbody2D>().AddForce(Vector3.right * knockBackDistance);
			//rigidbody2D.AddForce(Vector3.up * knockBackDistance);
		}
		else if(!left)
		{
			GetComponent<Rigidbody2D>().AddForce(-Vector3.right * knockBackDistance);
			//rigidbody2D.AddForce(Vector3.up * knockBackDistance);
		}
		print ("knocked back");
		
		_health.enemyHurt = false;
	}

	//stop, telegraph attack, attack, then return to follow sequence
	//once within range, repeat
	public IEnumerator AttackPlayer()
	{
		_anim.SetTrigger ("WindUp");
		speed = 0;
		attacking = true;
		yield return new WaitForSeconds (0.5f);
		speed = attackSpeed;
		_anim.SetTrigger("Attack");
		AttackSound ();
		yield return new WaitForSeconds (1f);
		attacking = false;
		yield return new WaitForSeconds (1f);
	}

	void OnDrawGizmos()
	{
		Gizmos.color = color;
		Gizmos.DrawCube(transform.position,new Vector2(size,size));
	}
}
