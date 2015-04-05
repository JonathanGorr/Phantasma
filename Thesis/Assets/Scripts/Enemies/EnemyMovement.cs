using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour {

	public float speed = .3f;
	
	// Update is called once per frame
	void Update () {

		GetComponent<Rigidbody2D>().velocity = new Vector2(this.transform.localScale.x, 0) * speed;
	
	}
}
