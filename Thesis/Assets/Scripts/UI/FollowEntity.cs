using UnityEngine;
using System.Collections;

public class FollowEntity : MonoBehaviour {

	public Transform target;
	public float damping = 1;
	public float lookAheadFactor = 3;
	public float lookAheadReturnSpeed = 0.5f;
	public float lookAheadMoveThreshold = 0.1f;
	
	private float offsetZ;
	private Vector3 lastTargetPosition;
	private Vector3 currentVelocity;
	private Vector3 lookAheadPos;
	private Transform myTransform;

	[HideInInspector]
	public bool spiritGet;

	private Animator _anim;
	private Player _player;

	//facing
	private bool left;
	private Vector2 faceLeft;
	private Vector2 faceRight;

	public float timeTilBored = 8f;
	private float t;

	// Use this for initialization
	void Start () {
		lastTargetPosition = target.position;
		offsetZ = (transform.position - target.position).z;
		transform.parent = null;
		myTransform = transform;
		_anim = GetComponentInChildren<Animator>();
		_player = target.GetComponent<Player>();

		//flipping
		faceLeft = new Vector2(transform.localScale.x, transform.localScale.y);
		faceRight = new Vector2(-transform.localScale.x, transform.localScale.y);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(target != null)
		{
			//player is on the right side
			if(target.position.x < myTransform.position.x)
			{
				left = true;
			}
			//if the player is left
			else if(target.position.x > myTransform.position.x)
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
					if(Mathf.Abs(_player.normalizedHorizontalSpeed) <= 0.1f)
					{
						Mathf.Clamp(t += Time.fixedDeltaTime, 0, 10);

						if(t >= timeTilBored)
						{
							_anim.SetInteger("AnimState", 1);
						}
					}

					// only update lookahead pos if accelerating or changed direction
					float xMoveDelta = (target.position - lastTargetPosition).x;
					
					bool updateLookAheadTarget = Mathf.Abs(xMoveDelta) > lookAheadMoveThreshold;
					
					if (updateLookAheadTarget) {
						lookAheadPos = lookAheadFactor * Vector3.right * Mathf.Sign(xMoveDelta);
					} else {
						lookAheadPos = Vector3.MoveTowards(lookAheadPos, Vector3.zero, Time.deltaTime * lookAheadReturnSpeed);	
					}
					
					Vector3 aheadTargetPos = target.position + lookAheadPos + Vector3.forward * offsetZ;
					Vector3 newPos = Vector3.SmoothDamp(myTransform.position, aheadTargetPos, ref currentVelocity, damping);
					
					myTransform.position = newPos;
					
					lastTargetPosition = target.position;
				}
			}
			else//play bored animation
				_anim.SetInteger("AnimState", 1);
		}
	}
}
