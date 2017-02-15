using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Weapons { Empty, SwordShield, Spear, Bow }
public class Weapon : MonoBehaviour {

	[Header("Weapon")]
	public Entity _entity;
	[HideInInspector] public PlayerInput _input;
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
	public int damage = 1;

	[Header("Stamina Costs")]
	public float regularAttackStaminaDrain = 1f;
	public float heavyAttackStaminaDrain = 2f;

	[Header("Delays")]
	public float delay = .1f;

	public virtual void OnEnable()
	{
		_input = GameObject.Find("_LevelManager").GetComponent<PlayerInput>();
		rayLength = Vector3.Distance(w_Base.position, w_tip.position);
	}

	public virtual void Update()
	{
		
	}
}
