using UnityEngine;
using System.Collections;

public class Bow : Weapon {

	private Vector3 gravity = new Vector3(0f,-0.64f,0f);
	private Vector2 direction = new Vector2(2,2);

	public GameObject _bodySprite;
	public GameObject arrowPrefab;

	public float speed;
	public float quickShotSpeed = 20f;
	public float min;
	public float max;

	[Header("Local Refs")]
	public Player _player;
	public CharacterController2D _controller;
	public LineRenderer lineRenderer;
	public WeaponController _switcher;

	private PlayerInput _input;
	public string sortingLayer = "Player";
	public int sortingNumber = -1;
	private Vector3 target;
	private bool ready = true;
	[HideInInspector] public Ray ray;

	private Vector2 dir;
	private float angle;

	//resolution/vertex count of line
	public int numSteps = 20;
	//time?
	public float timeDelta; // for example

	void Awake()
	{
		_input = GameObject.Find ("_LevelManager").GetComponent<PlayerInput> ();

		lineRenderer.sortingLayerName = sortingLayer;
		lineRenderer.sortingOrder = sortingNumber;
	}

	//these things come after the animator; overwrite the animator transforms
	void LateUpdate()
	{
		if(_switcher.IsWeapon(Weapons.Bow))
		{
			if(_controller.isGrounded)
			{
				if(_input._aiming)
				{
					//target is the players world position plus the axis
					if(_input._controller)
					{
					target = new Vector3(_bodySprite.transform.position.x + _input._axisHorizontal, _bodySprite.transform.position.y + _input._axisVertical, 0);

					//the direction of the projectile is the position difference between the target and origin
					dir = target - _bodySprite.transform.position;
					}
					
					//target is the mouse pos
					else
					{
						target = ray.direction;

						//the direction of the projectile is the mousePos
						dir = ray.direction;
					}

					angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

					//the vector for the players facing
					Vector3 direction = new Vector3();

					//facing left
					if(_player.facingLeft)
					{
						direction = Vector3.back;
						angle -= 180;
						angle = Mathf.Clamp(angle, min, max); //none of these values seem to clamp
					}

					//facing right
					else
					{
						direction = Vector3.forward;
						angle = Mathf.Clamp(angle, -15, 80);
					}

					//rotate body sprite
										_bodySprite.transform.rotation = Quaternion.AngleAxis(angle, direction);
				}
			}
		}
	}

	public override void Update()
	{
		//if the bow is out...
		if(_switcher.IsWeapon(Weapons.Bow))
		{
			ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));

			//if not jumping or falling...
			if(_controller.isGrounded)
			{
				//if aiming...
				if(_input._aiming)
				{
					Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);

					if(ready)
					{
						//controller
						if(_input._controller)
						{
							Cursor.visible = false;
							//lock linerender to either side of player, based on facing
							if(_player.facingLeft)
								direction.x = Mathf.Clamp(_input._axisHorizontal * 2, -2, 0);
							else
								direction.x = Mathf.Clamp(_input._axisHorizontal * 2, 0, 2);

							direction.y = Mathf.Clamp(_input._axisVertical * 2f + 1.5f, 1f, 3f);
						}

						//no controller
						else
						{
							Cursor.visible = true;

							//lock linerender to either side of player, based on facing
							if(_player.facingLeft)
								direction.x = Mathf.Clamp(ray.direction.x * 2 - .05f, -2, 0);
							else
								direction.x = Mathf.Clamp(ray.direction.x * 2 - .05f, 0, 2);
							
							direction.y = Mathf.Clamp(ray.direction.y * 2 + 2f, 0, 3);
						}

						//if aiming and shoot, shoot
						if(_input._attack) StartCoroutine(AimShot());
					}

					UpdateTrajectory(transform.position, direction, gravity);
				}

				else
				{
					Cursor.visible = false;

					//if not aiming or moving and ready, quickshot
					if(_player.normalizedHorizontalSpeed < 0.05f)
					{
						//turn off line rendering
						if(lineRenderer) lineRenderer.numPositions = 0;

						if(ready)
						{
							//if attack, shoot an arrow
							if(_input._attack) StartCoroutine(QuickShot());
						}
					}
				}
			}
		}
	}

	IEnumerator AimShot()
	{
		//TODO: instantiate in a ienumerator so that theres a delay each time an arrow is instantiated
		Quaternion rotation = Quaternion.Euler( 0, 0, Mathf.Atan2 ( direction.y, direction.x ) * Mathf.Rad2Deg );
		GameObject projectile = (GameObject) Instantiate(arrowPrefab, transform.position, rotation);
		//set damage
		projectile.GetComponent<PlayerAttack>().BlockingAttack();
		projectile.GetComponent<Rigidbody2D>().velocity = direction * speed;

		ready = false;
		yield return new WaitForSeconds(_switcher.weapon.delay);
		ready = true;
	}

	IEnumerator QuickShot()
	{
		//left or right based on facing
		Vector2 arrowDirection = new Vector2();

		//flipping
		if(!_player.facingLeft)
			arrowDirection = Vector2.right;
		else
			arrowDirection = -Vector2.right;
		
		//rotate arrow by velocity
		Quaternion rotation = Quaternion.Euler( 0, 0, Mathf.Atan2 ( arrowDirection.y, arrowDirection.x ) * Mathf.Rad2Deg );
		//instantiate
		GameObject projectile = (GameObject) Instantiate(arrowPrefab, transform.position, rotation);
		projectile.GetComponent<Rigidbody2D>().velocity = arrowDirection * quickShotSpeed;
		//set damage
		projectile.GetComponent<PlayerAttack>().r1Attack();

		ready = false;
		yield return new WaitForSeconds(_switcher.weapon.delay);
		ready = true;
	}

	void UpdateTrajectory(Vector3 initialPosition, Vector3 initialVelocity, Vector3 gravity)
	{
		timeDelta = 1f / initialVelocity.magnitude;

		if(lineRenderer) lineRenderer.numPositions = numSteps;
		
		Vector3 position = initialPosition;
		Vector3 velocity = initialVelocity;

		for (int i = 0; i < numSteps; ++i)
		{
			lineRenderer.SetPosition(i, position);
			position += velocity * timeDelta + 0.5f * gravity * timeDelta * timeDelta;
			velocity += gravity * timeDelta;
		}
	}
}
