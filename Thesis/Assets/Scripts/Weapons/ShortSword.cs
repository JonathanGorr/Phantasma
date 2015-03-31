using UnityEngine;
using System.Collections;

public class ShortSword : Weapon {

	void Awake()
	{
		weaponNumber = 1;

		attackDamage = 1;
		overheadStrikeDamage = 2;
		shieldedDamage = 1;
		speed = 5;

		weaponName = "Shortsword";
		weaponClass = "Sword";
		description = "This is a short sword. It is fast, but has a limited range.";

		rank = 0;
	}
	/*
	void OnGUI()
	{
		GUI.Label(());
	}
	*/
}