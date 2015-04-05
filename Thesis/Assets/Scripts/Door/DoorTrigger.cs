using UnityEngine;
using System.Collections;

public class DoorTrigger : MonoBehaviour {

	public Door door;
	public bool ignoreTrigger;

	void OnTriggerEnter2D(Collider2D target)
	{
		//tests for a value at the beginning of the function,
		//if the value yields true, return/exit out of function;
		//door.open() will not be called
		if (ignoreTrigger)
			return;

		if (target.gameObject.tag == "Player") 
		{
			door.Open ();
		}
	}

	void OnTriggerExit2D(Collider2D target)
		{

		if (ignoreTrigger)
			return;

		if(target.gameObject.tag == "Player")
		{
			door.Close();
		}

	}

	public void Toggle(bool value)
	{
		if (value)
			door.Open();
		else
			door.Close ();
	}

	void OnDrawGizmos()
	{
		Gizmos.color = ignoreTrigger ? Color.gray : Color.green;

		var bc2d = GetComponent<BoxCollider2D> ();
		var bc2dPos = bc2d.transform.position;
		var newPos = new Vector2 (bc2dPos.x + bc2d.offset.x, bc2dPos.y + bc2d.offset.y);
		Gizmos.DrawWireCube (newPos, new Vector2 (bc2d.size.x, bc2d.size.y));
	}
}
