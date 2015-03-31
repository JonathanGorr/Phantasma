using UnityEngine;
using System.Collections;

public class GhostAscend : MonoBehaviour {

	public float speed = .6f;
	
	// Update is called once per frame
	void Update () {

		rigidbody2D.velocity = new Vector2(0, this.transform.localScale.y) * speed;
	
	}
}
