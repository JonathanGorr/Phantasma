using UnityEngine;
using System.Collections;

/// <summary>
/// The behavior of an arrow shot from a bow.
/// Responsible for rotating arrow rigidbody and raycasting to hit objects.
/// </summary>
public class Arrow : MonoBehaviour {

	Transform myTransform;				//cache my transform
	public Entity myEntity;				//What entity shot me?
	public FadeOutSprite _fade;			//The component used to fade out this arrow after hit.
	public TrailRenderer trail;			//the trail renderer attached to this arrow
	public LayerMask pierceLayers;		//the gameobjects which this arrow will pass through
	public LayerMask stickLayers;		//the gameobjects which this arrow will embed into realistically( flesh, wood, etc )
	public Rigidbody2D rbody;			//this arrow's rigidbody.
	public SpriteRenderer rend;			//this arrow's sprite renderer.
	public float stickDepth = .5f; 		//how far does this arrow embed into it's targets?
	public float hitForce = 300;		//how much force does this attribute to hit objects( physics objects like pots and chandeliers )
	public float staggerAmt = 1;		//how many stagger points does this attibute to an enemies stagger meter?
	bool airborn = true;				//is this arrow flying through the air currently?
	bool canDamage = true;				//has this arrow hit anything; can it still damage?

	int damage;							//how much damage does this arrow do?
	/// <summary>
	/// Public damage get/setter.
	/// </summary>
	/// <value>The damage.</value>
	public int Damage
	{
		set { damage = value; }
	}

	void OnEnable()
	{
		//cache transform
		myTransform = transform;
		//set my trail to the player's trail
		trail.sortingLayerName = "Player"; //TODO: grab sorting layer from entity spriterenderer
		trail.sortingOrder = -1; //set to render behind entity //TODO: do this with a shader order instead?
		pierceLayers |= stickLayers; //add all stick layers to our pierceLayers
	}

	/// <summary>
	/// Destroy this arrow if it goes offscreen.
	/// </summary>
	void OnBecameInvisible()
	{
		//cannot damage if invisible
		canDamage = false;
		//remove enemies from detection
		pierceLayers &= ~LayerMask.NameToLayer("Enemies");
		stickLayers &= ~LayerMask.NameToLayer("Enemies");
	}

	void FixedUpdate()
	{
		if(airborn) Rotate(); //only rotate if this arrow is flying through the air
		if(!canDamage) return; //do not proceed if we already hit something

		RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, transform.right, .1f, pierceLayers);
		for(int i=0; i<hits.Length; i++)
		{
			//if this hit is null, continue to next collider in hit array
			if(hits[i].collider == null) continue;

			//deal damage
			if(hits[i].transform.GetComponent<Health>())
				hits[i].transform.GetComponent<Health>().TakeDamage(myEntity, damage);

			//adds a hit force to physics objects
			if(hits[i].transform.GetComponent<Rigidbody2D>())
				hits[i].transform.GetComponent<Rigidbody2D>().AddForceAtPosition(rbody.velocity.normalized * hitForce, hits[0].point);

			//triggers physics objects to destroy their joints and collapse();
			if(hits[i].transform.GetComponent<PhysicsTrigger>())
				hits[i].transform.GetComponent<PhysicsTrigger>().Collapse();

			//adds stagger points to any objects that have the stagger meter component
			if(hits[i].collider.GetComponent<StaggerMeter>())
				hits[i].collider.GetComponent<StaggerMeter>().AddStagger(staggerAmt);
			
			//stick to gameobject of stick layers hit by this arrow
			if(StaticMethods.IsInLayerMask(stickLayers, hits[i].collider.gameObject))
			{
				//play a hit sound effect at hit location
				SFX.Instance.PlayFX("arrow_" + hits[i].collider.tag, hits[i].point);
				//we can no longer damage anything
				canDamage = false;
				//we are no longer airborn
				airborn = false;
				//stop being affected by gravity or other objects
				rbody.isKinematic = true;
				//local scale = the scale of the hit object; scales arrows to objects that are transform.scaled up or down.
				myTransform.localScale = new Vector3(
				1/(hits[i].transform.localScale.x < 0 ? -hits[i].transform.localScale.x : hits[i].transform.localScale.x),
				1/hits[i].transform.localScale.y, 
				1);
				//move this arrow into the hit gameobject
				myTransform.Translate(rbody.velocity.normalized * stickDepth);
				//set position to hit location.
				myTransform.position = hits[i].point;
				//halt movement
				rbody.velocity = Vector2.zero;
				//set the parent of this arrow's transform to that which it hit
				myTransform.parent = hits[i].transform;
				//destroy my trail
				Destroy(trail);
				//start the fade out coroutine
				_fade.Fade();
				//stop iterating through hits array
				break;
			}
		}
	}

	/// <summary>
	/// Handles the rotation of the arrow while airborn; rotates with velocity vector.
	/// </summary>
	void Rotate()
	{
		//direction vector is velocity of arrow
		Vector3 dir = rbody.velocity;
		//transform vector into angle
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		//set arrow rotation by this angle
		myTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}
}