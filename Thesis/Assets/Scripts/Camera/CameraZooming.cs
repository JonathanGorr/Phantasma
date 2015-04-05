using UnityEngine;
using System.Collections;

public class CameraZooming : MonoBehaviour {

	public float distanceMin = 3.0f;
	public float distanceMax = 10.0f;

	public float zoomSpeed = 1.0f;

	public float distance = 10f;
	public float sensitivity = 0.5f;

	public bool zoomable;
	private bool _aiming;
	private float _axisVertical;
	private float _axisHorizontal;

	private WeaponSwitcher _switcher;

	void Awake()
	{
		_switcher = GameObject.Find ("_LevelManager").GetComponent<WeaponSwitcher>();
	}

	void Update()
	{
		_aiming = Input.GetButton ("360_LeftBumper");
		//clamp vertical looking to ground and above
		_axisVertical = Mathf.Clamp(Input.GetAxis ("360_RightStickVertical"), 0, 0.3f);
		//clamp horizontal looking to where the player is always visible
		_axisHorizontal = Mathf.Clamp(Input.GetAxis("360_RightStickHorizontal"),-0.6f, 0.6f);
	}

	void FixedUpdate()
	{
		//TODO: make the camera zoom in with the field of view change to change the depth of the image ;)

		GetComponent<Camera>().fieldOfView = distance;

		if(_aiming == false && zoomable)
		{
			distance = Mathf.Clamp(distance -
                       Input.GetAxis("360_RightStickVertical") * zoomSpeed,
                       distanceMin,
                       distanceMax);
		}
		//if aiming, and the bow is selected
		else if(_aiming && _switcher.currentWeapon == 3)
		{
			GetComponent<Camera>().transform.position =
						new Vector3(GetComponent<Camera>().transform.position.x + _axisHorizontal * sensitivity,
                        	GetComponent<Camera>().transform.position.y + _axisVertical * sensitivity,
				            GetComponent<Camera>().transform.position.z);
		}
	}
}
