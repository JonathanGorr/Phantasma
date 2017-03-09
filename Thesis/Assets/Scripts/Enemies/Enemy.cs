using UnityEngine;
using System.Collections;

public class Enemy : Entity {

	[Header("Enemy")]
	public LayerMask layers;
	public bool canRotate = false;
	public enum EnemyState { Patrol, Chase, Search }
	public EnemyState _AIState = EnemyState.Patrol;

	[HideInInspector] public Entity target;
	[HideInInspector] public CameraController cam;

	[HideInInspector] public Color debugColor;

	[HideInInspector] public Facing startFacing;
	[HideInInspector] public Vector2 startLocation;

	[Header("Enemy References")]
	public SpriteRenderer rend;
	public Rigidbody2D rbody;

	[Header("Ranges")]
	public float distance;
	public float chaseRange = 7f;
	public float attackRange = 2f;
	public float sightRadius = 5f;
	public float tooCloseRange = 1f;
	public float searchTime = 3f;
	public float minDistance = 2f;

	public float Distance() //returns the distance between me and my target
	{
		distance = Vector2.Distance(myTransform.position, target.myTransform.position);
		return distance;
	}

	public override void Awake()
	{
		base.Awake();
		startLocation = myTransform.position;
		startFacing = facing;
		cam = Camera.main.GetComponent<CameraController>();

		StartCoroutine("StateMachine");
	}

	public virtual Entity CircleCast() //casts a circle to detect targets
	{
		Collider2D col = Physics2D.OverlapCircle(transform.position, chaseRange, layers);
		if(col != null) 
		{
			return col.GetComponent<Entity>();
		}
		else return null;
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

	public virtual IEnumerator Patrol()
	{
		while(_AIState == EnemyState.Patrol)
		{
			yield return null;
		}
	}

	public virtual IEnumerator Chase()
	{
		while(_AIState == EnemyState.Chase)
		{
			yield return null;
		}
	}

	public virtual IEnumerator Search()
	{
		while(_AIState == EnemyState.Search)
		{
			yield return null;
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position, sightRadius);
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, chaseRange);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, attackRange);
	}
}
