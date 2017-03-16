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
	Vector2 velocity;
	public LayerMask offensiveLayers;

	public Vector2 Velocity
	{
		get { return velocity; }
		set { velocity = value; }
	}

	void Awake()
	{
		if(!myTransform) myTransform = transform;
	}

	//adds enemies to hurtable layermask on player's deflect
	public void SetOffensiveToEnemies()
	{
		offensiveLayers |= (1 << LayerMask.NameToLayer("Enemies"));
	}

	public void StartFlight(Vector2 v)
	{
		velocity = v;
		StartCoroutine(Fly());
	}

	void OnBecameInvisible()
	{
		Destroy(this.gameObject);
	}

	IEnumerator Fly()
	{
		while(true)
		{
			myTransform.position += (Vector3)velocity * flightSpeed * Time.fixedDeltaTime;
			yield return null;
		}
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if(canHarm)
		{
			if(offensiveLayers.IsInLayerMask(col.gameObject))
			{
				if(col.GetComponent<Entity>())
				{
					if(col.GetComponent<Entity>().combatState != CombatState.Blocking)
					{
						if(col.GetComponent<Health>())
						{
							canHarm = false;
							col.GetComponent<Health>().TakeDamage(myEntity, fireballDamage);
						}
					}
				}
			}
		}
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		//explode on impact with ground
		Instantiate(explosionPrefab, col.contacts[0].point, Quaternion.identity);
		//destroy this fireball
		Destroy(this.gameObject);
	}
}
