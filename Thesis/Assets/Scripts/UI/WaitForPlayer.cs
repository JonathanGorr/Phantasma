using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Waits for player before starting initialization( looking for it)
/// </summary>

public class WaitForPlayer : MonoBehaviour {

	public LevelManager _manager;
	[HideInInspector] public string currentSceneName;

	void Awake()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;	
	}

	public virtual void Start()
	{
	}

	public virtual void OnSceneLoaded(Scene scene, LoadSceneMode m)
	{
		_manager = GameObject.Find("_LevelManager").GetComponent<LevelManager>();
		currentSceneName = scene.name;
		StartCoroutine(Initialize(scene));
	}

	public virtual IEnumerator Initialize(Scene scene)
	{
		while(_manager.Player == null) yield return null;
	}
}
