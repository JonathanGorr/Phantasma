using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Weapons { Empty, SwordShield, Spear, Bow }
public class Weapon : MonoBehaviour {

	[Header("Weapon")]
	public Entity _entity;
	public ParticleSystem impactPS;
	public string title;
	public string description;
	public Weapons weapon;
	public Transform w_Base;
	public Transform w_tip;
	[HideInInspector] public float rayLength;

	[Header("Settings")]
	public bool canLook = true;
	public bool canBlock = false;
	public bool canRoll = true;
	public bool canBackStep = true;
	[HideInInspector] public int damage = 1;

	[Header("Damages")]
	public int regularAttackDamage = 1;
	public int specialAttackDamage = 2;
	public int slideAttackDamage = 2;
	public int dropAttackDamage = 3;

	[Header("Stamina Costs")]
	public float regularAttackStaminaDrain = 1f;
	public float specialAttackStaminaDrain = 2f;
	public float slideAttackStaminaDrain = 2f;
	public float dropAttackStaminaDrain = 3f;
	public float blockAttackStaminaDrain = 2f;

	[Header("Stagger Amounts")]
	public float regularAttackStaggerAmount = 1f;
	public float specialAttackStaggerAmount = 2f;
	public float slideAttackStaggerAmount = 2f;
	public float dropAttackStaggerAmount = 3f;
	public float blockAttackStaggerAmount = 1f;

	[Header("Delays")]
	public float delay = .1f;

	public virtual void OnEnable()
	{
		rayLength = Vector3.Distance(w_Base.position, w_tip.position);
	}

	public virtual void Update(){}

	public void OnTriggerEnter2D(Collider2D col)
	{
		if(col.CompareTag("Wood"))
		{
			impactPS.Emit(5);
		}
	}

	int Damage
	{
		get
		{
			if(_entity.attackState.inState) 			
				return regularAttackDamage;
			else if(_entity.specialAttackState.inState) 
				return specialAttackDamage;
			else 
				return 1;
			//drop attack
			//slide attack
			//etc
		}
	}

	[Header("Hit Detection")]
	public LayerMask hitLayers;
	public AnimationMethods animMethods;
	List<Collider2D> ignore = new List<Collider2D>();

	/// <summary>
	/// Raycasts from the base to the tip of weapons to detect enemies and destroy physics objects.
	/// </summary>
	void FixedUpdate()
	{
		if(weapon == Weapons.Empty) return;

		Vector2 direction = _entity.facing == Facing.left ? -w_Base.right : w_Base.right;
		Debug.DrawRay(w_Base.position, direction);
		if(animMethods.attacking)//is in attack animation
		{
			RaycastHit2D[] hits = Physics2D.RaycastAll(w_Base.position, direction, rayLength, hitLayers);
			if(hits.Length == 0) return;
			for(int i=0; i<hits.Length; i++)
			{
				if(hits[i].collider == null) continue; //continue to next hit if this one is null
				if(ignore.Contains(hits[i].collider)) continue; //if this collider is being ignored until animation is done, pass to next

				if(hits[i].collider.GetComponent<Health>())
				{
					//TODO: get animation and select damage amount based on that
					hits[i].collider.GetComponent<Health>().TakeDamage(_entity, Damage);
				}
				if(hits[i].collider.GetComponent<StaggerMeter>()) 
				{
					//TODO: get animation and select stagger amount based on that
					hits[i].collider.GetComponent<StaggerMeter>().AddStagger(regularAttackStaggerAmount);
				}
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
