using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {

	//associate animation with specific weapon
	public int weaponNumber;

	//damages
	public float attackDamage;
	public float overheadStrikeDamage;
	public float shieldedDamage;

	//qualities
	public string weaponName;//name of weapon
	public string weaponClass; //class of weapon: sword, lance, scimitar, scyth, halberd, bow, etc
	public string description; //details about it
	public int    rank; //its position on the chain of evolution
	public float  speed;//how fast the weapon swing speed is
}
