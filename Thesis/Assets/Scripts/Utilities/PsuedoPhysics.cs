using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterController;

public class PsuedoPhysics : MonoBehaviour {

	public CharacterController2D _controller;
	public Rigidbody2D _rbody;
	public LayerMask physicsLayer;
	public Transform feet;
	Rigidbody2D affectedBody;
	public float angleLimit = 30;
	public float lerpSpeed = 10;
	public float angle;
	float lastAngle;
	Vector2 normal;
	RaycastHit2D hit;

	void FixedUpdate()
	{
		if(!_controller.isGrounded) return;
		Debug.DrawRay(feet.position, -transform.up);
		hit = Physics2D.Raycast(feet.position, -transform.up, .5f, physicsLayer);
		if(hit.collider == null) return;
		normal = hit.normal;

		//if rigidbody, perform certain instructions
		if(hit.collider.GetComponent<Rigidbody2D>())
		{
			//get the hit's rbody
			if(hit.collider.GetComponent<Rigidbody2D>() != affectedBody) affectedBody = hit.collider.GetComponent<Rigidbody2D>();
			affectedBody.AddForceAtPosition(Vector3.down * _rbody.mass, hit.point);
		}
	}

	void LateUpdate()
	{
		if(hit.collider != null) 
		{
			angle = (Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg) - 90;
			transform.SetParent(hit.transform);
		}
		else
		{
			if(transform.parent != null) transform.SetParent(null);
		}

		//rotate while angle is not greater than 
		if(angle > -30 && angle < 30)
		{
			transform.rotation = Quaternion.AngleAxis(Mathf.LerpAngle(lastAngle, angle, Time.fixedDeltaTime * lerpSpeed), Vector3.forward);
			lastAngle = angle;
		}
		else //lerp back if exceeds angle limits
		{
			LerpToNoRotation();
		}

		//lerp back if no hit
		if(hit.collider == null && transform.rotation.eulerAngles.z != 0)
		{
			LerpToNoRotation();
		}

		lastAngle = transform.rotation.eulerAngles.z;
	}

	void LerpToNoRotation()
	{
		transform.rotation = Quaternion.AngleAxis(Mathf.LerpAngle(lastAngle, 0, Time.fixedDeltaTime * lerpSpeed), Vector3.forward);
	}
}