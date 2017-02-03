using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CameraFinishedPanning : MonoBehaviour {

	public Animator _anim;
	PlayerInput _input;
	GameObject _eventSystem;
	GameObject title;
	private bool done = false;

	void Awake()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode m)
	{
		if(scene.name == "Menu")
		{
			_input = GameObject.Find("_LevelManager").GetComponent<PlayerInput>();
			_eventSystem = GameObject.Find ("EventSystem");
			title = GameObject.Find("Title");
			title.SetActive(false);
			if (_eventSystem) _eventSystem.SetActive (false);
			_anim.SetTrigger("Pan");
		}
	}

	void Update()
	{
		if(!_input) return;
		if(!done)
		{
			//if(_input._jump)
			if(_input._anyKeyDown)
			{
				_anim.SetTrigger("Skip");
			}
		}
	}

	public void EnableControl()
	{
		title.SetActive(true);
		_eventSystem.SetActive (true);
		done = true;
	}
}
