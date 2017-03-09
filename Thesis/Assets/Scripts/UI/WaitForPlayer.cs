using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Waits for player before starting initialization( looking for it)
/// </summary>

public class WaitForPlayer : MonoBehaviour {

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
		currentSceneName = scene.name;
		StartCoroutine(Initialize(scene));
	}

	public virtual IEnumerator Initialize(Scene scene)
	{
		while(Player.Instance == null) yield return null;
	}
}
