using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAttractor : MonoBehaviour {

	public PlayerInfo playerInfo;
	public LayerMask layer;
	public float radius = 2.5f;
	public float itemMoveSpeed = 5f;
	public int itemID;

	//contains found items detected by circle cast
	public List<Collectable> foundItems = new List<Collectable>();

	void FixedUpdate()
	{
		//find items
		Collider2D col = Physics2D.OverlapCircle(transform.position, radius, layer);
		if(col != null)
		{
			if(col.GetComponent<Rigidbody2D>())
			{
				if(!foundItems.Contains(col.GetComponent<Collectable>()))
				{
					//col.enabled = false;
					foundItems.Add(col.GetComponent<Collectable>());
				}
			}
		}

		//attract items
		if(foundItems.Count == 0) return;

		//vacuum suck items towards the player
		foreach(Collectable c in foundItems)
		{
			if(c != null)
			{
				//effectively ignore until can attract
				if(!c.canAttract) continue;

				Vector2 direction = (Vector2)transform.position - (Vector2)c.transform.position;
				if(direction.sqrMagnitude > 0.1f) //while too far away
				{
					c.rbody.MovePosition(c.rbody.position + direction * itemMoveSpeed * Time.deltaTime);
				}
				else //close enough, "consume" item
				{
					playerInfo.AddItem(c.itemID);
					foundItems.Remove(c);
					Destroy(c.gameObject);
					break;
				}
			}
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, radius);
	}
}
