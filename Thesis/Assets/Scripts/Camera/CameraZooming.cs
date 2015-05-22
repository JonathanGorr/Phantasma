using UnityEngine;
using System.Collections;

public class CameraZooming : MonoBehaviour {

	public float distanceMin = 3.0f;
	public float distanceMax = 10.0f;

	public float zoomSpeed = 1.0f;

	public float distance = 10f;
	public float sensitivity = 0.5f;

	public bool zoomable;

	//components
	private WeaponSwitcher _switcher;
	private PlayerInput _input;
	private ShootArrow _shoot;

	void Awake()
	{
		_shoot = GameObject.Find ("_Player").GetComponent<ShootArrow> ();
		_input = GameObject.Find ("_LevelManager").GetComponent<PlayerInput> ();
		_switcher = GameObject.Find ("_LevelManager").GetComponent<WeaponSwitcher>();
	}

	void FixedUpdate()
	{
		GetComponent<Camera>().fieldOfView = distance;

		if(!_input._aiming && zoomable)
		{
			distance = Mathf.Clamp(distance -
                       Input.GetAxis("360_RightStickVertical") * zoomSpeed,
                       distanceMin,
                       distanceMax);
		}

		//if aiming, and the bow is selected
		if(_input._aiming && _switcher.currentWeapon == 3)
		{
			if(_input._controller)
			{
				GetComponent<Camera>().transform.position = new Vector3
					(GetComponent<Camera>().transform.position.x + _input._axisHorizontal * sensitivity,
	        		GetComponent<Camera>().transform.position.y + _input._axisVertical * sensitivity / 2,
	            	GetComponent<Camera>().transform.position.z);
			}
			else
			{
				GetComponent<Camera>().transform.position = new Vector3
				(GetComponent<Camera>().transform.position.x + _shoot.ray.direction.x * sensitivity,
				 GetComponent<Camera>().transform.position.y + _shoot.ray.direction.y * sensitivity,
				 GetComponent<Camera>().transform.position.z);
			}
		}
	}
}
