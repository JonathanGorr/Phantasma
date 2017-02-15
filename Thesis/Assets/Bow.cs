using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : Weapon {

	SFX _sfx;
	LevelManager _manager;
	public WeaponController _weaponController;
	public Player _player;
	public AnimationMethods _animMethods;
	public GameObject indicatorPrefab;
	public GameObject arrowPrefab;
	public Ray ray;
	bool aiming = false;
	bool ready = true;

	[Header("Trajectory")]
	private Transform arrowIndicator;
	private CanvasGroup arrowCG;
	public float maxSpeed = 15;
	public Vector3 gravity;
	private Vector2 direction;
	public float indicatorDistance = 5;
	Vector3 diff;
	float lookRotation;
	float normalizedDistance = 0;
	float lastRotation;

	[Header("Line")]
	public LineRenderer line;
	private string sortingLayer = "Player";
	private int sortingNumber = -1;
	public int lineResolution;

	public Vector3 Direction
	{
		get { return direction; }
	}

	public override void OnEnable()
	{
		if(!_manager)
		{
			_manager = GameObject.Find("_LevelManager").GetComponent<LevelManager>();
			_sfx = _manager.GetComponent<SFX>();
			_input = _manager.GetComponent<PlayerInput>();
			line.sortingLayerName = sortingLayer;
			line.sortingOrder = sortingNumber;
		}

		_input.onL1 += StartAim;
	}

	void OnDisable()
	{
		_input.onL1 -= StartAim;
	}

	void StartAim()
	{	
		if(_manager.paused) return;
		StartCoroutine("Aim");
		Rotate();
	}

	public override void Update()
	{
		base.Update();
		//set blocking bool
		_entity._anim.SetBool("Blocking", _entity.combatState == CombatState.Blocking);

		//Blocking and Strong Attack
		if(_entity._controller.isGrounded)
		{
			if(_entity.combatState == CombatState.Blocking)
			{
				_entity.SetSpeed (_entity.blockSpeed);
			}
			else if(_entity.combatState == CombatState.Attacking)
			{
				_entity.SetSpeed (_entity.blockSpeed);
			}
			else
			{
				_entity.SetSpeed (_entity.walkSpeed);
			}
		}
	}

	IEnumerator Aim()
	{
		float progress = 0;

		while(_input.L1Down)
		{
			if(!aiming)
			{
				aiming = true;
				_sfx.PlayFX("draw_arrow", transform.position);
				if(!arrowIndicator) 
				{
					arrowIndicator = Instantiate(indicatorPrefab, _player.myTransform).transform;
					arrowCG = arrowIndicator.GetComponent<CanvasGroup>();
				}
				arrowIndicator.gameObject.SetActive(true);
				progress = 0;
			}
			else
			{
				Rotate();
				direction = new Vector2(_input.RAnalog.x, _input.RAnalog.y);
				_player.SetFacing(_input.RAnalog.x < 0 ? true : false);
				Debug.DrawRay(transform.position, direction);

				// set the arrow speed to the progress of the draw animation;
				// shooting at animation end would yield highest speed( and power? ).
				AnimatorStateInfo currentState = _player._anim.GetCurrentAnimatorStateInfo(0);
				if(currentState.IsName("Block"))
				{
					if(progress < 1) progress = Mathf.Clamp(currentState.normalizedTime, 0, 1);

					while(_input.R1Down && !ready) yield return null;
					ready = true;

					if(_input.R1Down && _animMethods.canFire && ready && progress > .5f && _player._stamina.Ready)
					{
						_player._stamina.UseStamina(_player.lightAttackDrain);
						_animMethods.CantFire();
						FireArrow(progress);
						_player._anim.SetBool("Blocking", false);
						aiming = false;
						ready = false;
						yield return null;
					}
				}
			}
			yield return null;
		}

		//set off
		_player._anim.SetBool("Blocking", false);
		line.numPositions = 0;
		arrowIndicator.gameObject.SetActive(false);
		aiming = false;
	}

	void FireArrow(float progress)
	{
		float speed = Mathf.SmoothStep(0, maxSpeed, progress);
		_sfx.PlayFX("fire_arrow", transform.position);
		GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.Euler(new Vector3(0,0, lookRotation + 90)));
		arrow.GetComponent<Rigidbody2D>().velocity = direction * speed;
		Arrow a = arrow.GetComponent<Arrow>();
		a.Damage = damage;
		a.SFX = _sfx;
	}

	void Rotate()
	{
		arrowIndicator.position = LookPosition();
		diff =  _entity.body.position - arrowIndicator.position;
		lookRotation = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
		normalizedDistance = direction.sqrMagnitude;
		arrowIndicator.rotation = Quaternion.Euler(new Vector3(0,0, lookRotation + 90));
		arrowCG.alpha = Mathf.Lerp(0, 1, normalizedDistance);
	}

	public Vector3 LookPosition()
	{
		return Vector3.ClampMagnitude(direction * indicatorDistance, indicatorDistance) + _entity.body.position;
	}

	void UpdateTrajectory(Vector3 initialPosition, Vector3 initialVelocity, Vector3 gravity)
	{
	    float timeDelta = 1.0f / initialVelocity.magnitude; // for example
		line.SetVertexCount(lineResolution);
	 
	    Vector3 position = initialPosition;
	    Vector3 velocity = initialVelocity;
		for (int i = 0; i < lineResolution; ++i)
	    {
			line.SetPosition(i, position);
	        position += velocity * timeDelta + 0.5f * gravity * timeDelta * timeDelta;
	        velocity += gravity * timeDelta;
	    }
	}
}
