using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : Weapon {

	public WeaponController _weaponController;
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
		PlayerInput.onL1 += StartAim;
	}

	void OnDisable()
	{
		PlayerInput.onL1 -= StartAim;
	}

	void StartAim()
	{	
		if(PauseMenu.paused) return;
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

		while(PlayerInput.Instance.L1Down)
		{
			if(!aiming)
			{
				aiming = true;
				SFX.Instance.PlayFX("draw_arrow", transform.position);
				if(!arrowIndicator) 
				{
					arrowIndicator = Instantiate(indicatorPrefab, _entity.myTransform).transform;
					arrowCG = arrowIndicator.GetComponent<CanvasGroup>();
				}
				arrowIndicator.gameObject.SetActive(true);
				progress = 0;
			}
			else
			{
				Rotate();
				direction = new Vector2(PlayerInput.Instance.RAnalog.x, PlayerInput.Instance.RAnalog.y);
				Debug.DrawRay(transform.position, direction);

				// set the arrow speed to the progress of the draw animation;
				// shooting at animation end would yield highest speed( and power? ).
				AnimatorStateInfo currentState = _entity._anim.GetCurrentAnimatorStateInfo(0);
				if(currentState.IsName("Block"))
				{
					if(progress < 1) progress = Mathf.Clamp(currentState.normalizedTime, 0, 1);

					while(PlayerInput.Instance.R1Down && !ready) yield return null;
					ready = true;

					if(PlayerInput.Instance.R1Down && animMethods.canFire && ready && progress > .5f && _entity._stamina.Ready)
					{
						animMethods.CantFire();
						FireArrow(progress);
						_entity._anim.SetBool("Blocking", false);
						aiming = false;
						ready = false;
						yield return null;
					}
				}
			}
			yield return null;
		}

		//set off
		_entity._anim.SetBool("Blocking", false);
		line.numPositions = 0;
		arrowIndicator.gameObject.SetActive(false);
		aiming = false;
	}

	void FireArrow(float progress)
	{
		float speed = Mathf.SmoothStep(0, maxSpeed, progress);
		SFX.Instance.PlayFX("fire_arrow", transform.position);
		GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.Euler(new Vector3(0,0, lookRotation + 90)));
		arrow.GetComponent<Rigidbody2D>().velocity = (_entity.facing == Facing.left ? -transform.right : transform.right) * speed * normalizedDistance;
		Arrow a = arrow.GetComponent<Arrow>();
		a.myEntity = _entity;
		a.Damage = damage;
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
