using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsTrigger : MonoBehaviour {

	public Joint2D[] joints;
	public float explosionForce = 6;

	void Awake()
	{
		joints = GetComponentsInChildren<Joint2D>();
	}

	public void Collapse()
	{
		for(int i=0;i<joints.Length;i++)
		{
			if(joints[i] == null) continue;
			joints[i].breakForce = 1;
			joints[i].GetComponent<Collider2D>().isTrigger = false;
			joints[i].gameObject.AddComponent<FadeOutSprite>();
			joints[i].GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.value, Random.value) * explosionForce, ForceMode2D.Impulse);
		}
	}
}
