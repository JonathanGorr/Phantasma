using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {

	public float speed = 30;

	void Awake()
	{
		int number = Random.Range (0, 2);

		float left = -speed;
		float right = speed;

		if(Random.Range(0,2) == 1)
			speed = left;
		else
			speed = right;
	}

	void FixedUpdate()
	{
		transform.Rotate(Vector3.back, speed * Time.deltaTime);
	}
}
