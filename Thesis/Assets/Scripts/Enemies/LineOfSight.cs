using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Line of sight:
/// used by enemies to detect entities in their line of sight
/// the max range of sight is determined by the circle collider radius
/// </summary>

public class LineOfSight : MonoBehaviour {

	Transform myTransform;
	public Enemy myEnemy;
	public Transform head;
	public float angleRange = 5;
	public bool seen = false;
	float distance;
	public Entity target;
	public LayerMask rayLayers;

	//used to tell subscribers when an entity has been lost
	public event Action<Entity> foundEntity;
	public event Action<Entity> lostEntity;

	void Awake()
	{
		myTransform = transform;
	}

	//If the player is visible, chase in enemy script
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

    //returns the distance between me and the target entity
    public float Distance
    {
    	get { return Vector2.Distance(myTransform.position, target.myTransform.position); }
    }

	void OnTriggerStay2D(Collider2D col)
    {
		if(col.CompareTag("Player"))
        {
        	if(!target)
        	{
				target = col.GetComponent<Entity>();
			}

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
		if(col.CompareTag("Player"))
        {
        	if(target)
        	{
        		if(myEnemy._AIState == Enemy.EnemyState.Chase)
        		{
					if(lostEntity != null) lostEntity(target);
		        	target = null;
	        	}
        	}
        }
    }

	public bool PlayerInFieldOfView()
    {
        // check if the player is within the enemy's field of view
        // this is only checked if the player is within the enemy's sight range

        // find the angle between the enemy's 'forward' direction and the player's location and return true if it's within 65 degrees (for 130 degree field of view)
		Vector2 directionToPlayer = target.Center - head.position; // represents the direction from the enemy to the player    

		Vector2 direction = myEnemy.facing == Facing.left ? -Vector2.right : Vector2.right;
		Debug.DrawRay(head.position, direction, Color.yellow); // a line drawn in the Scene window equivalent to the enemy's field of view centre
        
        // calculate the angle formed between the player's position and the centre of the enemy's line of sight
		float angle = Vector2.Angle(directionToPlayer, direction);
        
        // if the player is within 65 degrees (either direction) of the enemy's centre of vision (i.e. within a 130 degree cone whose centre is directly ahead of the enemy) return true
		if (angle < angleRange)
            return true;
        else
            return false;
    }

	public bool PlayerHiddenByObstacles()
    {
		distance = Vector2.Distance(head.position, target.Center);
		RaycastHit2D[] hits = Physics2D.RaycastAll(head.position, target.Center - head.position, distance, rayLayers);
		Debug.DrawLine(head.position, Physics2D.Raycast(head.position, target.Center - head.position, distance, rayLayers).point, Color.blue);
     	
        foreach (RaycastHit2D hit in hits)
        {
            // ignore the enemy's own colliders (and other enemies)
            if (hit.transform.tag == "Enemy")
                continue;
            
            // if anything other than the player is hit then it must be between the player and the enemy's eyes (since the player can only see as far as the player)
            if (hit.transform.tag != "Player")
            {
                return true;
            }
        }

        // if no objects were closer to the enemy than the player return false (player is not hidden by an object)
        return false; 
    }
}
