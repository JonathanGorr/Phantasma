using UnityEngine;
using System;

/// <summary>
/// Used by enemies to detect targets in their cone of 2d sight.
/// </summary>
public class LineOfSight : MonoBehaviour {

	Transform myTransform;			//store my transform
	public Enemy myEnemy;			//store my enemy script
	public Transform head;			//store my head transform
	public float angleRange = 5;	//what vision angle extent can we detect?
	public Entity target;			//my entity target
	public LayerMask rayLayers;		//the gameobjects I can detect

	//used to tell subscribers when an entity has been lost
	public event Action<Entity> foundEntity;
	public event Action<Entity> lostEntity;

	float distance;					//how far away is my target?
	/// <summary>
	/// Returns the distance between me and my target.
	/// </summary>
	public float Distance
	{
		get { return Vector2.Distance(myTransform.position, target.myTransform.position); }
	}

	void Awake()
	{
		//cache transform
		myTransform = transform;
	}

	/// <summary>
	/// Can I see my target?
	/// </summary>
	/// <returns><c>true</c> if this instance can player be seen; otherwise, <c>false</c>.</returns>
    public bool CanPlayerBeSeen()
    {
        // we only need to check visibility if the player is within the enemy's visual range
        if (target)
        {
            if(PlayerInFieldOfView())
                return (!PlayerHiddenByObstacles());
            else
                return false;
        }
        else
        {
            // always false if the player is not within the enemy's range
            return false;
        }
    }

	void OnTriggerStay2D(Collider2D col)
    {
    	//while we are in the player trigger
		if(col.CompareTag("Player"))
        {
        	//set target if none exists
        	if(!target) target = col.GetComponent<Entity>();
			//find player if visible
			if(myEnemy._AIState != Enemy.EnemyState.Chase)
			{
				if(CanPlayerBeSeen())
				{
					if(foundEntity != null) foundEntity(target);
				}
        	}
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
    	//if we exited the player trigger
		if(col.CompareTag("Player"))
        {
        	//if I have a target
        	if(target)
        	{
        		//exit chase state if in
        		if(myEnemy._AIState == Enemy.EnemyState.Chase)
        		{
					if(lostEntity != null) lostEntity(target);
	        	}
				//lose target
				target = null;
        	}
        }
    }

    /// <summary>
	/// Checks if the player is within the enemy's field of view
	/// This is only checked if the player is within the enemy's sight range
    /// </summary>
    /// <returns><c>true</c>, if player is in field of view, <c>false</c> otherwise.</returns>
	public bool PlayerInFieldOfView()
    {
        // find the angle between the enemy's 'forward' direction and the player's location and return true if it's within 65 degrees (for 130 degree field of view)
		Vector2 directionToPlayer = target.Center - head.position;    
		// represents the direction from the enemy to the player
		Vector2 direction = myEnemy.facing == Facing.left ? -Vector2.right : Vector2.right;
		// a line drawn in the Scene window equivalent to the enemy's field of view centre
		Debug.DrawRay(head.position, direction, Color.yellow);
        // calculate the angle formed between the player's position and the centre of the enemy's line of sight
		float angle = Vector2.Angle(directionToPlayer, direction);
        // if the player is within 65 degrees (either direction) of the enemy's centre of vision (i.e. within a 130 degree cone whose centre is directly ahead of the enemy) return true
		if (angle < angleRange) { return true; }
        else { return false; }
    }

    /// <summary>
    /// Checks if any obstacles are obscuring view of the player.
    /// </summary>
    /// <returns><c>true</c>, if player is hidden by objects, <c>false</c> otherwise.</returns>
	public bool PlayerHiddenByObstacles()
    {
    	//TODO: improvement would be to ray check head, feet and center in case these are revealed
    	//checks distance between my head and my target's center
		distance = Vector2.Distance(head.position, target.Center);
		//cast a ray from my head to my target's center over target distance
		RaycastHit2D[] hits = Physics2D.RaycastAll(head.position, target.Center - head.position, distance, rayLayers);
		//draw a line to represent this ray
		Debug.DrawLine(head.position, Physics2D.Raycast(head.position, target.Center - head.position, distance, rayLayers).point, Color.blue);
		//check all rays
		for(int i=0; i<hits.Length; i++)
		{
			// ignore the enemy's own colliders (and other enemies)
			if(hits[i].transform.CompareTag("Enemy"))
			{
				//ignore
				continue;
			}
			// if anything other than the player is hit then it must be between the player...
			// and the enemy's eyes (since the player can only see as far as the player)...
			if(!hits[i].transform.CompareTag("Player"))
			{
				//invisible
				return true;
			}
		}
        // if no objects were closer to the enemy than the player return false (player is not hidden by an object)
        return false;
    }
}
