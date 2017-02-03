using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour {

	public Rigidbody2D rbody;

	void FixedUpdate()
	{
		Vector3 dir = rbody.velocity;
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		transform.parent = col.transform;
	}
}