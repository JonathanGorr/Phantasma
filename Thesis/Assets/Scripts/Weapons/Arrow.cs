using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour {

	public Entity myEntity;
	public FadeOutSprite _fade;
	public TrailRenderer trail;
	public LayerMask pierceLayers;
	public LayerMask stickLayers;
	public Rigidbody2D rbody;
	public SpriteRenderer rend;
	public float stickDepth = .5f;
	public float hitForce = 300;
	public float staggerAmt = 1;
	bool airborn = true;
	int damage;
	bool canDamage = true;

	public int Damage
	{
		set { damage = value; }
	}

	void OnEnable()
	{
		trail.sortingLayerName = "Player";
		trail.sortingOrder = -1;

		pierceLayers |= stickLayers;
	}

	void FixedUpdate()
	{
		if(airborn) Rotate();

		if(!canDamage) return;
		RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, transform.right, .1f, pierceLayers);
		for(int i=0; i<hits.Length; i++)
		{
			if(hits[i].collider == null) continue;

			//deal damage
			if(hits[i].transform.GetComponent<Health>())
				hits[i].transform.GetComponent<Health>().TakeDamage(myEntity, damage);

			//adds a hit force to physics objects
			if(hits[i].transform.GetComponent<Rigidbody2D>())
				hits[i].transform.GetComponent<Rigidbody2D>().AddForceAtPosition(rbody.velocity.normalized * hitForce, hits[0].point);

			if(hits[i].transform.GetComponent<PhysicsTrigger>())
				hits[i].transform.GetComponent<PhysicsTrigger>().Collapse();

			if(hits[i].collider.GetComponent<StaggerMeter>())
				hits[i].collider.GetComponent<StaggerMeter>().AddStagger(staggerAmt);
			
			//stick if a stickobject
			if(StaticMethods.IsInLayerMask(stickLayers, hits[i].collider.gameObject))
			{
				SFX.Instance.PlayFX("arrow_" + hits[i].collider.tag, hits[i].point);
				canDamage = false;
				airborn = false;
				rbody.isKinematic = true;
				transform.localScale = new Vector3(
				1/(hits[i].transform.localScale.x < 0 ? -hits[i].transform.localScale.x : hits[i].transform.localScale.x),
				1/hits[i].transform.localScale.y, 1);
				transform.Translate(rbody.velocity.normalized * stickDepth);
				transform.position = hits[i].point;
				rbody.velocity = Vector2.zero; //stop moving
				transform.parent = hits[i].transform;
				Destroy(trail);
				_fade.Fade();
				break;
			}
		}
	}

	void Rotate()
	{
		Vector3 dir = rbody.velocity;
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}
}