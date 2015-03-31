using UnityEngine;
using System.Collections;

public class Enemy : Entity {

	//offset of healthbar relative to enemy height/width
	public float offsetX;
	public float offsetY;

	//how much blood will drop on death
	public int bloodAmount;

	//instantiation params
	public GameObject deathInstance = null;
	public Vector2 deathInstanceOffset = new Vector2(0,0);

	public void DropBlood()
	{
		var pos = gameObject.transform.position;
#pragma warning disable 0219
		GameObject clone = Instantiate (deathInstance, new Vector3(pos.x + deathInstanceOffset.x, pos.y + deathInstanceOffset.y, 
        				   pos.z), Quaternion.identity) as GameObject;
#pragma warning restore 0219
	}
}
