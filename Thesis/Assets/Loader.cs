using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Loader : MonoBehaviour {

	public GameObject playerPrefab;
	public GameObject levelManagerPrefab;

	void Awake()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
		DontDestroyOnLoad(gameObject);
		if(!GameObject.FindWithTag("Player"))
		{
			GameObject p = Instantiate(playerPrefab);
			p.name = "_Player";
		}
		if(!GameObject.Find("_LevelManager"))
		{
			GameObject lm = Instantiate(levelManagerPrefab);
			lm.name = "_LevelManager";
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
