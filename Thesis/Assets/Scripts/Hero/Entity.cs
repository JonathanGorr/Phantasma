using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using CharacterController;

public enum CombatState { Idle, Blocking, Attacking, Rolling, BackStepping, Jumping }
public class Entity : MonoBehaviour, IEntity {

	///<summary>
	///	Root class for all variables shared by humans, npcs, and enemies
	///

	[Header("Entity")]
	public string title;
	public string Title
	{
		get { return title; }
		set { title = value; }
	}
	public string description;
	public string Description
	{
		get { return description; }
		set { description = value; }
	}

	public Transform body;
	[HideInInspector] private bool facingLeft = false;
	public bool FacingLeft
	{
		get { return facingLeft; }
		set { facingLeft = value; }
	}
	float lastRotation;
	Vector2 lookDirection = Vector2.right;
	public Vector2 LookDirection
	{
		get { return lookDirection; }
		set { lookDirection = value; }
	}

	public Health _health;
	public Transform myTransform;
	public Animator _anim;
	[HideInInspector] public SFX _sfx;
	public ParticleSystem _footDust;
	public CharacterController2D _controller;
	public CombatState combatState;

	[Header("Speeds")]
	public float walkSpeed = 3f;
	public float runSpeed = 6f;
	public float runSpeedHeavy = 4f;
	public float blockSpeed = 0f;
	[HideInInspector] public float speed;
	public Vector2 rotOffset;
	public float rotLerpSpeed = 2;

	[Header("Stamina Drains")]
	public Stamina _stamina;
	public float jumpDrain = 1;
	public float rollDrain = 1;
	public float backStepDrain = 1;
	public float lightAttackDrain = 1;
	public float heavyAttackDrain = 1;
	public float blockedHitDrain = 1;

	[HideInInspector]
	public float normalizedHorizontalSpeed = 0;

	public virtual void Awake()
	{
		if(!myTransform) myTransform = transform;
	}

	public virtual void Start()
	{
		_sfx = GameObject.Find("_LevelManager").GetComponent<SFX>();
	}

	public virtual void OnHurt()
	{
	}

	public virtual void Heal()
	{
	}

	public virtual void RotateBody()
	{
		float lookRotation = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
		float normalizedDistance = lookDirection.sqrMagnitude;

		if(normalizedDistance > 0.05f)
		{
			float angle = facingLeft ? (180 + lookRotation + rotOffset.x) : (lookRotation + rotOffset.y);
			//angle = Mathf.Clamp(angle, -30, 80);
			//angle = Mathf.Lerp(lastRotation, angle, Time.deltaTime * rotLerpSpeed);
			body.rotation = Quaternion.Euler(new Vector3(
			body.rotation.eulerAngles.x, body.rotation.eulerAngles.y,
			angle));

			lastRotation = body.rotation.eulerAngles.z;
		}
		else
		{
			body.rotation = Quaternion.Euler(new Vector3(0,0, lastRotation));
		}
	}

	public virtual void LateUpdate()
	{
		if(combatState == CombatState.Blocking)
		{
			RotateBody();
		}
	}

	public virtual void Attack()
	{
	}

	public virtual void Block()
	{
	}

	public virtual void Roll()
	{
	}

	public virtual void BackStep()
	{
	}

	public virtual void Slide()
	{
		_sfx.PlayFX("slide", myTransform.position);
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
