using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour {

	public Transform myTransform;
	private Transform target;
	private Vector2 offset;

	public Transform Target
	{
		get { return target; }
		set { target = value; }
	}

	public Vector2 Offset
	{
		get { return offset; }
		set { offset = value; }
	}

	void Awake()
	{
		if(!myTransform) myTransform = transform;
	}

	public void OnDeath()
	{
		//print("called");
		Destroy(this.gameObject);
	}

	void Update()
	{
		if(target == null) return;
		myTransform.position = target.position + (Vector3)offset;
	}
}
