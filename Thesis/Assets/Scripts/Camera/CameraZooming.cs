using UnityEngine;
using System.Collections;

public class CameraZooming : MonoBehaviour {

	public float distanceMin = 3.0f;
	public float distanceMax = 10.0f;

	public float zoomSpeed = 1.0f;

	public float distance = 10f;
	public float sensitivity = 0.5f;

	public bool zoomable;

	Camera cam;

	//components
	private WeaponController _switcher;
	private PlayerInput _input;
	private Bow _shoot;

	void Start()
	{
		cam = GetComponent<Camera>();
		_shoot = GameObject.Find ("_Player").GetComponent<Bow> ();
		_input = GameObject.Find ("_LevelManager").GetComponent<PlayerInput> ();
		_switcher = GameObject.Find ("_Player").GetComponent<WeaponController>();
	}

	void FixedUpdate()
	{
		if(!_switcher.IsWeapon(Weapons.Bow)) return;

		cam.fieldOfView = distance;

		if(!_input._aiming && zoomable)
		{
			distance = Mathf.Clamp(distance -
		       Input.GetAxis("360_RightStickVertical") * zoomSpeed,
		       distanceMin,
		       distanceMax);
		}

		//if aiming, and the bow is selected
		if(_input._aiming)
		{
			if(_input._controller)
			{
				cam.transform.position = new Vector3
					(cam.transform.position.x + _input._axisHorizontal * sensitivity,
	        		cam.transform.position.y + _input._axisVertical * sensitivity / 2,
	            	cam.transform.position.z);
			}
			else
			{
				cam.transform.position = new Vector3
				(cam.transform.position.x + _shoot.ray.direction.x * sensitivity,
				 cam.transform.position.y + _shoot.ray.direction.y * sensitivity,
				 cam.transform.position.z);
			}
		}
	}
}
