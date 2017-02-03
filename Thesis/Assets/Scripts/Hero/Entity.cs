using UnityEngine;
using System.Collections;

public enum CombatState { Idle, Blocking, Attacking, Rolling, BackStepping, Jumping }
public class Entity : MonoBehaviour {

	///<summary>
	///	Root class for all variables shared by humans, npcs, and enemies
	///

	[Header("Entity")]
	public Transform myTransform;
	public Animator _anim;
	public ParticleSystem _footDust;
	public CharacterController2D _controller;
	public string entityName;//what is the name of the entity?
	public string description; //some information to tell the player
	public CombatState combatState;

	[Header("Speeds")]
	public float walkSpeed = 3f;
	public float runSpeed = 6f;
	public float runSpeedHeavy = 4f;
	public float blockSpeed = 0f;
	[HideInInspector] public float speed;

	[HideInInspector]
	public float normalizedHorizontalSpeed = 0;

	public virtual void Awake()
	{
		if(!myTransform) myTransform = transform;
	}

	public virtual void OnHurt()
	{
		
	}

	public virtual void OnDeath()
	{
			
	}

	public virtual void OnBecameInvisible()
	{
		
	}

	public virtual void OnBecameVisible()
	{
		
	}

	//takes a float speed num, changes speed based on input
	public virtual void SetSpeed(float num)
	{
		speed = num;
	}
}
