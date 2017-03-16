using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : Entity {

	[Header("Enemy")]
	public LineOfSight sight;
	public LayerMask layers;
	public bool canRotate = false;
	public enum EnemyState { Patrol, Chase, Search }
	public EnemyState _AIState = EnemyState.Patrol;

	[HideInInspector] public CameraController cam;

	[HideInInspector] public Color debugColor;

	[HideInInspector] public Facing startFacing;
	[HideInInspector] public Vector2 startLocation;

	[Header("Enemy References")]
	public SpriteRenderer rend;
	public Rigidbody2D rbody;

	[Header("Ranges")]
	public float distance = 99;
	public float attackRange = 2f;
	public float searchTime = 3f;
	public float minDistance = 2f;

	public DrawWireDisc[] ranges;

	public float Distance() //returns the distance between me and my target
	{
		distance = Vector2.Distance(myTransform.position, sight.target.myTransform.position);
		return distance;
	}

	#if UNITY_EDITOR
	void OnGUI()
	{
		if(!rend.isVisible) return;
		ranges[0].diameter = attackRange;
	}
	#endif

	public override void Awake()
	{
		base.Awake();

		#if UNITY_EDITOR
		//get all of our editor wire circles
		ranges[0].color = Color.red;
		ranges[0].diameter = attackRange;
		#endif

		startLocation = myTransform.position;
		startFacing = facing;
		cam = Camera.main.GetComponent<CameraController>();

		StartCoroutine("StateMachine");
	}

	public override void Subscribe()
	{
		sight.foundEntity += FoundTarget;
		sight.lostEntity += LostTarget;

		base.Subscribe();
	}

	public override void UnSubscribe()
	{
		sight.foundEntity -= FoundTarget;
		sight.lostEntity -= LostTarget;

		base.UnSubscribe();
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

	//called by the lineofSight script when the player enters the collider and is visible to the enemy
	public virtual void FoundTarget(Entity e)
	{
		if(_AIState == EnemyState.Chase) return;
		_AIState = EnemyState.Chase;
	}
	//called by the lineofSight script when the player has left the collider
	public virtual void LostTarget(Entity e)
	{
		_AIState = EnemyState.Search;
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
}
