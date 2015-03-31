using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour {

	//Root class for all variables shared by humans, npcs, and enemies

	//details
	public string entityName;//what is the name of the entity?
	public string description; //some information to tell the player
	public int age;//how old is the entity

	//stats
	public float health; //current health of the creature
	public float maxHealth; //max health
}
