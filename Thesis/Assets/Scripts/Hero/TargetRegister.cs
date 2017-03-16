using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Target register:
/// Responsible for adding targets to the camera target list
/// If a target exits the circle collider and remains exited for more than x seconds, remove from list
/// </summary>

public class TargetRegister : MonoBehaviour {

	public float waitToRemoveTarget = 2;

	void OnTriggerEnter2D(Collider2D col)
	{
		if(col.CompareTag("Enemy"))
		{
			CameraController.Instance.RegisterMe(col.transform);
		}
	}

	void OnTriggerExit2D(Collider2D col)
	{
		if(col.CompareTag("Enemy"))
		{
			CameraController.Instance.UnRegisterMe(col.transform);
		}
	}
}