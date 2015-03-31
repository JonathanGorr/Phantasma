using UnityEngine;
using System.Collections;

public class ArrowStick : MonoBehaviour {

	private Rigidbody2D _body;

	void Awake()
	{
		_body = GetComponent<Rigidbody2D>();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		_body.isKinematic = true;
		transform.position = other.transform.position;
		print ("freeze");
	}
}
