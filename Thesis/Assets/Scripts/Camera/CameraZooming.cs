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
	private LevelManager _manager;
	private WeaponController _switcher;
	private PlayerInput _input;

	void Start()
	{
		cam = GetComponent<Camera>();
		_manager = GameObject.Find ("_LevelManager").GetComponent<LevelManager>();
		_input = _manager.GetComponent<PlayerInput> ();
		_switcher = _manager.Player.GetComponent<WeaponController>();
	}

	void FixedUpdate()
	{
		if(!_switcher.IsWeapon(Weapons.Bow)) return;

		cam.fieldOfView = distance;

		//if aiming, and the bow is selected
		if(_input.L1Down && !_manager.paused)
		{
			cam.transform.position = new Vector3
				(cam.transform.position.x + _input.RAnalog.x * sensitivity,
        		cam.transform.position.y + _input.RAnalog.y * sensitivity / 2,
            	cam.transform.position.z);
		}
	}
}
