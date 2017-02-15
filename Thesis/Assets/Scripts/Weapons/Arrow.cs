using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour {

	SFX _sfx;
	public FadeOutSprite _fade;
	public TrailRenderer trail;
	public LayerMask pierceLayers;
	public LayerMask stickLayers;
	public Rigidbody2D rbody;
	public SpriteRenderer rend;
	public float stickDepth = .5f;
	public float hitForce = 300;
	bool airborn = true;
	int damage;
	bool canDamage = true;

	public SFX SFX
	{
		set { _sfx = value; }
	}
	public int Damage
	{
		set { damage = value; }
	}

	void OnEnable()
	{
		if(!_sfx) _sfx = GameObject.Find("_LevelManager").GetComponent<SFX>();
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
				hits[i].transform.GetComponent<Health>().TakeDamage(damage);

			//adds a hit force to physics objects
			if(hits[i].transform.GetComponent<Rigidbody2D>())
				hits[i].transform.GetComponent<Rigidbody2D>().AddForceAtPosition(rbody.velocity.normalized * hitForce, hits[0].point);

			//destroy pots
			if(hits[i].transform.GetComponent<Destructable>())
				hits[i].transform.GetComponent<Destructable>().Explode(hits[i].point);

			if(hits[i].transform.GetComponent<PhysicsTrigger>())
				hits[i].transform.GetComponent<PhysicsTrigger>().Collapse();

			//stick if a stickobject
			if(StaticMethods.IsInLayerMask(stickLayers, hits[i].collider.gameObject))
			{
				_sfx.PlayFX("arrow_" + hits[i].collider.tag, hits[i].point);
				canDamage = false;
				airborn = false;
				rbody.isKinematic = true;
				transform.localScale = new Vector3(1/hits[i].transform.localScale.x, 1/hits[i].transform.localScale.y, 1);
				transform.Translate(rbody.velocity.normalized * stickDepth);
				transform.position = hits[i].point;
				rbody.velocity = Vector2.zero;
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