using UnityEngine;
using System.Collections;

public class CameraFinishedPanning : MonoBehaviour {

	private GameObject _eventSystem;
	private GameObject _title;

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
		if (_title)
		{
			_title.SetActive(true);
			//_title.GetComponent<Animation>()["Title"].speed = Random.Range(0,2);
			_title.GetComponent<Animator>().Play ("Title");
		}
	}
}
