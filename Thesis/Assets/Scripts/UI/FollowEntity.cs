using UnityEngine;
using System.Collections;

public class FollowEntity : Entity {

	[Header("Follow")]
	public Entity target;
	public float damping = 1;
	public float lookAheadFactor = 3;
	public float lookAheadReturnSpeed = 0.5f;
	public float lookAheadMoveThreshold = 0.1f;
	
	private float offsetZ;
	private Vector3 lastTargetPosition;
	private Vector3 currentVelocity;
	private Vector3 lookAheadPos;

	[HideInInspector]
	public bool spiritGet;

	//facing
	private bool left;
	private Vector2 faceLeft;
	private Vector2 faceRight;

	public float timeTilBored = 8f;
	private float t;

	// Use this for initialization
	void Awake () 
	{
		//flipping
		faceLeft = new Vector2(transform.localScale.x, transform.localScale.y);
		faceRight = new Vector2(-transform.localScale.x, transform.localScale.y);
	}

	public void SetTarget(Entity t)
	{
		target = t;
		offsetZ = (transform.position - target.myTransform.position).z;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(target == null) return;

		//player is on the right side
		if(target.myTransform.position.x < myTransform.position.x)
		{
			left = true;
		}
		//if the player is left
		else if(target.myTransform.position.x > myTransform.position.x)
		{
			left = false;
		}
		//flip
		if(left)
		{
			myTransform.localScale = faceLeft;
		}
		else if(!left)
		{
			myTransform.localScale = faceRight;
		}
		
		else
		{
			t = 0;
			_anim.SetInteger("AnimState", 0);
		}

		if(spiritGet)
		{
			if(target != null)
			{
				//if the player is afk for more than 10 seconds, play the bored animation
				if(Mathf.Abs(target.normalizedHorizontalSpeed) <= 0.1f)
				{
					Mathf.Clamp(t += Time.fixedDeltaTime, 0, 10);

					if(t >= timeTilBored)
					{
						_anim.SetInteger("AnimState", 1);
					}
				}

				// only update lookahead pos if accelerating or changed direction
				float xMoveDelta = (target.myTransform.position - lastTargetPosition).x;
				
				bool updateLookAheadTarget = Mathf.Abs(xMoveDelta) > lookAheadMoveThreshold;
				
				if (updateLookAheadTarget) {
					lookAheadPos = lookAheadFactor * Vector3.right * Mathf.Sign(xMoveDelta);
				} else {
					lookAheadPos = Vector3.MoveTowards(lookAheadPos, Vector3.zero, Time.deltaTime * lookAheadReturnSpeed);	
				}
				
				Vector3 aheadTargetPos = target.myTransform.position + lookAheadPos + Vector3.forward * offsetZ;
				Vector3 newPos = Vector3.SmoothDamp(myTransform.position, aheadTargetPos, ref currentVelocity, damping);
				
				myTransform.position = newPos;
				
				lastTargetPosition = target.myTransform.position;
			}
		}
		else//play bored animation
			_anim.SetInteger("AnimState", 1);
	}
}
