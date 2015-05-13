using UnityEngine;
using System.Collections;

public class CameraFinishedPanning : MonoBehaviour {

	private GameObject _eventSystem;
	private GameObject _title;
	private bool done;

	void Awake()
	{
		_title = GameObject.Find ("Title");
		_eventSystem = GameObject.Find ("EventSystem");

		if (_eventSystem) _eventSystem.SetActive (false);
		if(_title)
		{
			_title.SetActive(false);
		}
	}

	public void EnableControl()
	{
		if(_eventSystem) _eventSystem.SetActive (true);

		_title.SetActive(true);
		_title.GetComponent<Animator>().Play ("Title");
	}
}
