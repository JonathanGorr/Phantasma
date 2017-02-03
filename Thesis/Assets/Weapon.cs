using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Weapons { Empty, SwordShield, Spear, Bow }
public class Weapon : MonoBehaviour {

	public Entity _entity;
	public string title;
	public string description;
	public Weapons weapon;

	[Header("Settings")]
	public bool canBlock = false;
	public bool canRoll = true;
	public bool canBackStep = true;
	public bool canMove = true;

	[Header("Delays")]
	public float delay = .1f;

	public virtual void OnEnable()
	{
		
	}

	public virtual void Update()
	{
		
	}
}
