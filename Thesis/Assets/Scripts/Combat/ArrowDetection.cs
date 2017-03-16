using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Arrow detection:
/// Used by certain entities to detect whether an arrow is in view
/// </summary>

public class ArrowDetection : MonoBehaviour {

	public Enemy myEnemy;

	void OnTriggerEnter2D(Collider2D col)
	{
		if(col.CompareTag("Arrow"))
		{
			if(myEnemy._AIState == Enemy.EnemyState.Chase)
			{
				myEnemy.SetFacing(col.transform.position.x < myEnemy.myTransform.position.x ? Facing.left : Facing.right);
				myEnemy.Block();
			}
		}
	}
}
