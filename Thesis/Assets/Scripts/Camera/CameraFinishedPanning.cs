using UnityEngine;
using System.Collections;

public class CameraFinishedPanning : MonoBehaviour {

	private GameObject _eventSystem;

	void Awake()
	{
		_eventSystem = GameObject.Find ("EventSystem");

		if (_eventSystem != null)
			_eventSystem.SetActive (false);
	}

	public void EnableControl()
	{
		if(_eventSystem != null)
			_eventSystem.SetActive (true);
	}
}
