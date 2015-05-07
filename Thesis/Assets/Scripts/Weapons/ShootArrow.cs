using UnityEngine;
using System.Collections;

public class ShootArrow : MonoBehaviour {

	private Vector3 gravity = new Vector3(0f,-0.64f,0f);
	private Vector2 direction = new Vector2(2,2);

	public GameObject arrowPrefab;

	public float 
		speed,
		quickShotDelay = 1f,
		quickShotSpeed = 20f,
		min,
		max;

	private bool
		_shoot,
		drawn;

	[HideInInspector]
	public bool _aim;

	private float 
		_axisVertical,
		_axisHorizontal;

	private Player _player;
	private CharacterController2D _controller;
	private LineRenderer lineRenderer;
	public string sortingLayer = "Player";
	public int sortingNumber = -1;
	private GameObject _body;
	private Vector3 target;
	private int index;
	private WeaponSwitcher _switcher;
	private string[] joysticks;

	//resolution/vertex count of line
	public int numSteps = 20;
	//time?
	public float timeDelta; // for example

	void Awake()
	{
		_player = GetComponent<Player>();
		_body = GameObject.Find("_Player/BodyParts/Body");
		_switcher = GameObject.Find("_LevelManager").GetComponent<WeaponSwitcher>();
		lineRenderer = GetComponentInChildren<LineRenderer>();
		_controller = GetComponent<CharacterController2D>();

		if(lineRenderer != null)
		{
			lineRenderer.sortingLayerName = sortingLayer;
			lineRenderer.sortingOrder = sortingNumber;
		}
	}

	//inputs
	void Update()
	{
		//how many controller are plugged in?
		joysticks = Input.GetJoystickNames ();

		_shoot = Input.GetButtonDown ("360_RightBumper") || Input.GetKeyDown (KeyCode.J);

		if(joysticks.Length > 0)
		{
			_axisVertical = Input.GetAxis ("360_RightStickVertical");
			_axisHorizontal = Input.GetAxis ("360_RightStickHorizontal");
		}
		else if(joysticks.Length == 0)
		{
			_axisHorizontal = Input.GetAxis ("Horizontal");
			_axisVertical = Input.GetAxis ("Vertical");
		}

		//if the controller button is held, or the G button is held, while the bow is out, aiming is true
		_aim = Input.GetButton ("360_LeftBumper") || Input.GetKey (KeyCode.G) && index == 3;

		//target is the players world position plus the axis
		target = new Vector3(_body.transform.position.x + _axisHorizontal, _body.transform.position.y + _axisVertical, 0);

		index = _switcher.currentWeapon;
	}

	//these things come after the animator; overwrite the animator transforms
	void LateUpdate()
	{
		if(index == 3)
		{
			if(_controller.isGrounded)
			{
				if(_aim)
				{
					//the vector for the players facing
					Vector3 direction = new Vector3();

					//the direction of the projectile is the position difference between the target and origin
					Vector2 dir = target - _body.transform.position;
					float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

					if(_player.facingLeft == false)
					{
						direction = Vector3.forward;
						angle = Mathf.Clamp(angle, -15, 80);
					}
					else if(_player.facingLeft == true)
					{
						direction = Vector3.back;
						angle += 180;
						//angle = Mathf.Clamp(angle, min, max); //none of these values seem to clamp
					}

					//rotate body sprite
					_body.transform.rotation = Quaternion.AngleAxis(angle, direction);
				}
			}
		}
	}

	void FixedUpdate()
	{
		if(index == 3)
		{
			if(_controller.isGrounded)
			{
				if(_aim)
				{
					//lock linerender to either side of player, based on facing
					if(_player.facingLeft == false)
					{
						direction.x = Mathf.Clamp(_axisHorizontal * 2, 0, 2);
					}
					else if(_player.facingLeft == true)
					{
						direction.x = Mathf.Clamp(_axisHorizontal * 2, -2, 0);
					}

					//direction.x = _axisHorizontal * 2;
					direction.y = Mathf.Clamp(_axisVertical * 2f + 1.5f, 1f, 3f);

					UpdateTrajectory(transform.position, direction, gravity);//transform.position, transform.forward, gravity

					if(drawn)
					{
						if(_shoot)
						{
							//TODO: instantiate in a ienumerator so that theres a delay each time an arrow is instantiated
							Quaternion rotation = Quaternion.Euler( 0, 0, Mathf.Atan2 ( direction.y, direction.x ) * Mathf.Rad2Deg );
							GameObject projectile = (GameObject) Instantiate(arrowPrefab, transform.position, rotation);
							drawn = false;
							//set damage
							projectile.GetComponent<PlayerAttack>().BlockingAttack();
							projectile.GetComponent<Rigidbody2D>().velocity = direction * speed;
						}
					}
				}
				//if not aiming or drawn or moving and ready, quickshot
				else if(!_aim && !drawn && _player._ready == true && Mathf.Abs(_player.normalizedHorizontalSpeed) < 0.1f)
				{
					//turn off line rendering
					lineRenderer.SetVertexCount(0);

					if(_shoot)
						StartCoroutine(QuickShot());
				}
				else
					//turn off line rendering
					lineRenderer.SetVertexCount(0);
			}
		}
	}

	public IEnumerator QuickShot()
	{
		//left or right based on facing
		Vector2 arrowDirection = new Vector2();

		//flipping
		if(_player.facingLeft == false)
		{
			arrowDirection = Vector2.right;
		}
		else if(_player.facingLeft == true)
		{
			arrowDirection = -Vector2.right;
		}
		
		//rotate arrow by velocity
		Quaternion rotation = Quaternion.Euler( 0, 0, Mathf.Atan2 ( arrowDirection.y, arrowDirection.x ) * Mathf.Rad2Deg );
		//instantiate
		GameObject projectile = (GameObject) Instantiate(arrowPrefab, transform.position, rotation);
		drawn = false;
		projectile.GetComponent<Rigidbody2D>().velocity = arrowDirection * quickShotSpeed;
		//set damage
		projectile.GetComponent<PlayerAttack>().r1Attack();

		yield return new WaitForSeconds(quickShotDelay);
	}

	void UpdateTrajectory(Vector3 initialPosition, Vector3 initialVelocity, Vector3 gravity)
	{
		timeDelta = 1.0f / initialVelocity.magnitude;

		lineRenderer.SetVertexCount(numSteps);
		
		Vector3 position = initialPosition;
		Vector3 velocity = initialVelocity;
		for (int i = 0; i < numSteps; ++i)
		{
			lineRenderer.SetPosition(i, position);
			
			position += velocity * timeDelta + 0.5f * gravity * timeDelta * timeDelta;
			velocity += gravity * timeDelta;
		}
	}

	public void Drawn()
	{
		drawn = true;
	}
	
	public void NotDrawn()
	{
		drawn = false;
	}
}
