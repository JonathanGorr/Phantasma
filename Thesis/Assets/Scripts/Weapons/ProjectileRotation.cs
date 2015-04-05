using UnityEngine;
using System.Collections;

public class ProjectileRotation : MonoBehaviour {

	void FixedUpdate()
	{
		Vector3 dir = GetComponent<Rigidbody2D>().velocity;
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}
}
