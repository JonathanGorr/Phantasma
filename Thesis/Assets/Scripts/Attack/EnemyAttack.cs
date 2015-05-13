using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour {
	
	//import animator via reference
	private Animator _animator;
	//import Player script
	private Player _player;
	public int damage;
	public GameObject hitSprite;
	public bool readyToAttack = false;
	
	// Use this for initialization
	void Awake () {
		//get the animator component
		_animator = GetComponent<Animator> ();
	}

	public void Attack(int trigger)
	{
		_animator.SetTrigger ("" + trigger);
	}

	void Ready()
	{
		readyToAttack = true;
	}
	
	void NotReady()
	{
		readyToAttack = false;
	}
	
	void OnTriggerStay2D(Collider2D target)
	{
		if(target != null)
		{
			if (target.gameObject.tag == "Player")
			{
				//test if the player is ready to attack
				if(readyToAttack)
				{
					var _health = target.GetComponent<Health>();
					//instantiate hit sprite at collider
					Transform source = GetComponent<BoxCollider2D>().transform;
					_health.PlayerTakeDamage(damage);
					//instantiate hitsprite
					Instantiate(hitSprite, source.position, source.rotation);
					readyToAttack = false;
				}
			}
			else if (target.gameObject.tag == "Weapon")
			{
				//test if the player is ready to attack
				if(readyToAttack)
				{
					readyToAttack = false;
				}
			}
		}
		else
			print("target was destroyed");
	}

	//when the enemy is already dealt damage, dont keep dealing; once per swing
	void onTriggerExit2D (Collider2D target)
	{
		readyToAttack = false;
	}
}
