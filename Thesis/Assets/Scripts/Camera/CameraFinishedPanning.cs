using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CameraFinishedPanning : MonoBehaviour {

	public Animator _anim;
	GameObject _eventSystem;
	private bool done = false;

	void Awake()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode m)
	{
		_anim.enabled = false;
		done = false;

		if(scene.name == "Menu")
		{
			_anim.Rebind();
			_anim.enabled = true;

			_eventSystem = GameObject.Find ("EventSystem");
			_eventSystem.SetActive (false);

			_anim.SetTrigger("Pan");
		}
	}

	void Update()
	{
		if(!done)
		{
			if(Input.anyKeyDown)
			{
				_anim.SetTrigger("Skip");
			}
		}
	}

	public void EnableControl()
	{
		_eventSystem.SetActive (true);
		done = true;
	}
}
