using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour {

	public Transform myTransform;
	public Entity myEntity;
	bool canHarm = true;
	public GameObject explosionPrefab;
	public int fireballDamage = 1;
	public float flightSpeed = 10;
	Vector2 myDirection;

	void Awake()
	{
		if(!myTransform) myTransform = transform;
	}

	public void StartFlight(Vector2 direction)
	{
		myDirection = direction;
		StartCoroutine(Fly());
	}

	IEnumerator Fly()
	{
		while(true)
		{
			myTransform.position += (Vector3)myDirection * flightSpeed * Time.fixedDeltaTime;
			yield return null;
		}
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if(canHarm)
		{
			if(col.CompareTag("Player"))
			{
				canHarm = false;
				col.GetComponent<Health>().TakeDamage(myEntity, fireballDamage);
			}
		}
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		if(col.gameObject.CompareTag("Shield"))
		{
			//TODO: reflect things
			myDirection = Vector2.Reflect(myDirection, col.contacts[0].normal);
			return;
		}
		//explode on impact with ground
		Instantiate(explosionPrefab, col.contacts[0].point, Quaternion.identity);
		//destroy this fireball
		Destroy(this.gameObject);
	}
}
