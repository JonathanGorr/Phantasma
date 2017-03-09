using UnityEngine;
using System.Collections;

public class FollowEntity : MonoBehaviour {

	[Header("Follow")]
	public Transform myTransform;
	public Animator _anim;
	public Facing facing;
	public Entity target;
	public float damping = 1;
	public float lookAheadFactor = 3;
	public float lookAheadReturnSpeed = 0.5f;
	public float lookAheadMoveThreshold = 0.1f;
	public float offsetY = 2.5f;

	private float offsetZ;
	private Vector3 lastTargetPosition;
	private Vector3 currentVelocity;
	private Vector3 lookAheadPos;

	bool bored;
	public float timeTilBored = 8f;
	[SerializeField] private float t;

	Vector2 localScale;

	// Use this for initialization
	public void Awake () 
	{
		localScale = myTransform.localScale;
		_anim.SetInteger("AnimState", 1);
		//flipping
		if(!myTransform) myTransform = transform;
		facing = myTransform.localScale.x < 0 ? Facing.left : Facing.right;
	}

	public void SetTarget(Entity t)
	{
		target = t;
		offsetZ = (transform.position - target.myTransform.position).z;
	}

	void Bored()
	{
		//if the player is afk for more than 10 seconds, play the bored animation
		if(!target.Moving)
		{
			if(t < timeTilBored) t += Time.deltaTime;
			else 
			{
				if(!bored)
				{
					bored = true;
					_anim.SetInteger("AnimState", 1); 
				}
			}
		}
		else
		{
			t = 0;
			if(bored)
			{
				_anim.SetInteger("AnimState", 0);
				bored = false;
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(target == null) return;
		Bored();

		//player is on the right side
		if(target.myTransform.position.x < myTransform.position.x)
		{
			myTransform.localScale = new Vector2(-Mathf.Abs(localScale.x), localScale.y);
		}
		//if the player is left
		else if(target.myTransform.position.x > myTransform.position.x)
		{
			myTransform.localScale = new Vector2(-Mathf.Abs(localScale.x), localScale.y);
		}

		// only update lookahead pos if accelerating or changed direction
		float xMoveDelta = (target.myTransform.position - lastTargetPosition).x;
		
		bool updateLookAheadTarget = Mathf.Abs(xMoveDelta) > lookAheadMoveThreshold;
		
		if (updateLookAheadTarget) 
			lookAheadPos = lookAheadFactor * Vector3.right * Mathf.Sign(xMoveDelta);
		else 
			lookAheadPos = Vector3.MoveTowards(lookAheadPos, Vector3.zero, Time.deltaTime * lookAheadReturnSpeed);	
		
		Vector3 aheadTargetPos = target.myTransform.position + lookAheadPos + Vector3.forward * offsetZ + new Vector3(0, offsetY, 0);
		Vector3 newPos = Vector3.SmoothDamp(myTransform.position, aheadTargetPos, ref currentVelocity, damping);
		
		myTransform.position = newPos;
		
		lastTargetPosition = target.myTransform.position;
	}
}
