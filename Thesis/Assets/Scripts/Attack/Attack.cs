using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Attack:
/// The collider associated with a weapon.
/// </summary>

public class Attack : MonoBehaviour {

	public LayerMask hitLayers;
	public AnimationMethods animMethods;
	public Weapon weapon;
	List<Collider2D> ignore = new List<Collider2D>();

	/// <summary>
	/// Raycasts from the base to the tip of weapons to detect enemies and destroy physics objects.
	/// </summary>
	void FixedUpdate()
	{
		Debug.DrawRay(weapon.w_Base.position, weapon.w_Base.right);
		if(animMethods.attacking)//is in attack animation
		{
			RaycastHit2D[] hits = Physics2D.RaycastAll(weapon.w_Base.position, weapon.w_Base.right, weapon.rayLength, hitLayers);
			if(hits.Length == 0) return;
			for(int i=0; i<hits.Length; i++)
			{
				if(hits[i].collider == null) continue; //continue to next hit if this one is null
				if(ignore.Contains(hits[i].collider)) continue; //if this collider is being ignored until animation is done, pass to next

				if(hits[i].collider.GetComponent<Health>()) hits[i].collider.GetComponent<Health>().TakeDamage(weapon.damage);
				if(hits[i].collider.GetComponent<Destructable>()) hits[i].collider.GetComponent<Destructable>().Explode(hits[i].point);
				StartCoroutine(CanHarm(hits[i].collider)); //set this collider to be ignored so isn't hurt more than once per attack
			}
		}
	}

	/// <summary>
	/// Adds hit entity's collider to list while animation is playing.
	/// </summary>
	IEnumerator CanHarm(Collider2D col)
	{
		ignore.Add(col);
		while(animMethods.attacking) yield return null;
		ignore.Remove(col);
	}
}
