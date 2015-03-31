using UnityEngine;
using System.Collections;

public class Grunt : Enemy {

	void Awake()
	{
		//modify inherited variables/traits
		//qualities
		name = "Grunt";
		description = "Low-level enemy, low health, low attack rate and damage";
		age = 18;

		//stats
		maxHealth = 5;
		health = 5;
		bloodAmount = 5;
	}
}
