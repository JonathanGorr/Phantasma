using UnityEngine;
using System.Collections;

public class Collectable : MonoBehaviour 
{
	public Rigidbody2D rbody;
	public int itemID;
	public bool canAttract = false;

	void OnCollisionEnter2D(Collision2D col)
	{
		//can only attract after having hit the floor once
		canAttract = true;
	}
}
