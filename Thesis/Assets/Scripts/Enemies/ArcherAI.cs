using UnityEngine;
using System.Collections;

public class ArcherAI : MonoBehaviour {

	public Rigidbody2D projectile;
	public Transform target;
	private float delayAmount = 2.0f;
	public float forceMultiplier = 1f;
	private Transform myTransform;
	private Vector2 arrowVelocity;
	private float timeDelay = 0f;

	void Awake()
	{
		//cache it
		myTransform = transform;
	}

	void FixedUpdate()
	{
		if(Time.time > timeDelay)
		{
			Trajectory();
			timeDelay = Time.time + delayAmount;
		}
	}

	//used for calculating trajectory based on positions
	void Trajectory() {
		if(target != null)
		{
			//Solve for time
			float emitterYPos = myTransform.position.y; //Y distance from ground
			float Ay = Physics.gravity.y;

			//Solve for the initial X Velocity
			float targetXPos = target.position.x - myTransform.position.x;
			float targetYPos = target.position.y - myTransform.position.y;
			
			//Calculate the X and Z distances from the target
			float Vix = targetXPos * (Mathf.Abs(Ay / (2 * emitterYPos)));
			float Viz = targetYPos * (Mathf.Abs(Ay / (2 * emitterYPos)));

			Shoot(Vix, Viz);
		}
	}

	//create arrow, point it at target, shoot
	void Shoot(float x, float y)
	{
		Rigidbody2D newArrow = (Rigidbody2D) Instantiate(projectile, myTransform.position, myTransform.rotation);
		newArrow.name = "EnemyArrow";
		Vector2 dir = target.position - myTransform.position;
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		newArrow.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		//clamp the speed...
		float xSpeed = Mathf.Clamp (x * forceMultiplier, -20, 20);
		float ySpeed = Mathf.Clamp(y * forceMultiplier, -10, 10);
		arrowVelocity = myTransform.TransformDirection(new Vector3(xSpeed, ySpeed, 0));
		//assign the velocity to the clamped speed
		newArrow.GetComponent<Rigidbody2D>().velocity = arrowVelocity;
	}
}