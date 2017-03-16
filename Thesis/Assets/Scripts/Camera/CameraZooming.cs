using UnityEngine;
using System.Collections;

public class CameraZooming : WaitForPlayer {

	public float distanceMin = 3.0f;
	public float distanceMax = 10.0f;

	public float zoomSpeed = 1.0f;

	public float distance = 10f;
	public float sensitivity = 0.5f;

	Camera cam;

	Vector2 input;

	public override IEnumerator Initialize(UnityEngine.SceneManagement.Scene scene)
	{
		while(Player.Instance == null) yield return null;
		cam = GetComponent<Camera>();
	}

	void FixedUpdate()
	{
		if(Player.Instance._health.Dead) return;
		if(!Player.Instance) return;

		cam.fieldOfView = distance;
		//if aiming, and the bow is selected
		if(!PauseMenu.paused)
		{
			cam.transform.position = new Vector3
				(cam.transform.position.x + PlayerInput.Instance.RAnalog.x * sensitivity,
				cam.transform.position.y + PlayerInput.Instance.RAnalog.y * sensitivity / 2,
            	cam.transform.position.z);
		}
	}
}
