using UnityEngine;
using System.Collections;

public class Collectable : MonoBehaviour 
{
	public LayerMask layer;
	public float radius = .4f;
	public int checksPerSecond = 10;
	public int itemID;

	void OnEnable()
	{
		StartCoroutine(CircleCast());
	}

	IEnumerator CircleCast()
	{
		while(enabled)
		{
			Collider2D col = Physics2D.OverlapCircle(transform.position, radius, layer);
			if(col != null)
			{
				if(col.CompareTag("Player"))
				{
					col.GetComponent<PlayerInfo>().AddItem(itemID);
					Destroy(gameObject);
				}
			}
			yield return new WaitForSeconds(1.0f/checksPerSecond); //x per second
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, radius);
	}
}
