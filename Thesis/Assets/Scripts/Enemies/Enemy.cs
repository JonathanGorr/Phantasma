using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class is dedicated to the enemy state machine and behavior.
/// </summary>
public class Enemy : Entity {

	[Header("Enemy")]
	public LineOfSight sight;							//the compopnent responsible for detecting targetable entities
	public bool canRotate = false;						//can this enemy rotate to look at a target?
	public enum EnemyState { Patrol, Chase, Search }	//the state machine states
	public EnemyState _AIState = EnemyState.Patrol;		//the current state of this machine

	[HideInInspector] public Facing startFacing;		//save the start facing so we can return to it
	[HideInInspector] public Vector2 startLocation;		//save the start location so we can return to it

	[Header("Enemy References")]
	public SpriteRenderer rend;							//sprite renderer reference
	public Rigidbody2D rbody;							//rigidbody reference

	[Header("Ranges")]
	public float attackRange = 2f;						//how close the target needs to be to allow attacking
	public float searchTime = 3f;						//how long we wait after losing a target before reentering the patrol state
	public float minDistance = 2f;						//the distance at which we disable seeking movement

	void OnGUI()
	{
		if(!rend.isVisible) return;
	}

	public override void Awake()
	{
		base.Awake();

		#if UNITY_EDITOR
		//get all of our editor wire circles
		#endif

		startLocation = myTransform.position;
		startFacing = facing;

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
