using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

/// <summary>
/// Loader:
/// Used to instantiate and store references to important, non-destructable gameobjects like cameras, ui, players, etc
/// Keeps a registry of players for various utilities like the quest system
/// </summary>

public class Loader : MonoBehaviour {

	public bool spawnLevelManager = true;

	public GameObject levelManagerPrefab;

	void Awake()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
		DontDestroyOnLoad(gameObject);

		if(!GameObject.Find("_LevelManager") && spawnLevelManager)
		{
			GameObject lm = Instantiate(levelManagerPrefab);
			lm.name = "_LevelManager";
			DontDestroyOnLoad(lm);
		}
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode m)
	{
		if(scene.name == "Initialize")
		{
			//go to the next scene( Menu )
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
		}
	}
}
