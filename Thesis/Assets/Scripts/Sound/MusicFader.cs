using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// Music fader.
/// bg_music Plays a constant background song
/// event_music fades in a different dialog or event-specific, temporary theme to be later faded out
/// </summary>

public class MusicFader : MonoBehaviour {

	[Tooltip("Press J to start transition")] public bool debug;

	[Header("Audio Sources")]
	public AudioSource bg_Music;
	public AudioSource event_Music;

	[Header("Settings")]
	public bool resetEventMusicOnTransitionEnd = false;
	public float fadeSpeed = 1;
	bool done = false;
	bool transitioning = false;
	float t = 0;

	[Header("Themes")]
	public AudioClip menuTheme;
	public AudioClip forestTheme;
	public AudioClip battleTheme;
	public AudioClip motherTheme;
	public AudioClip fatherTheme;
	public AudioClip victoryTheme;
	public AudioClip bossTheme;

	void Awake()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	void Update()
	{
		if(debug)
		{
			if(Input.GetKeyDown(KeyCode.J))
			{
				FadeTo(battleTheme);
			}
		}
	}

	public void SetVolume(float v)
	{
		bg_Music.volume = v;
		event_Music.volume = v;
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode m)
	{
		if(bg_Music.isPlaying) 
		bg_Music.Stop();

		switch(scene.name)
		{
		case "Menu":
		bg_Music.clip = menuTheme;
		break;
		case "Start":
		bg_Music.clip = forestTheme;
		break;
		}

		bg_Music.Play();
	}

	public void FadeTo(AudioClip music)
	{
		if(event_Music.clip != music) event_Music.clip = music;
		if(!event_Music.isPlaying) event_Music.Play();
		if(transitioning) StopCoroutine("CrossFade");
		StartCoroutine("CrossFade");
	}

	IEnumerator CrossFade()
	{
		transitioning = true;
		done = !done; //toggle
		while((done && t < 1) || (!done && t > 0)) //while not 1...
		{
			t += (done ? Time.deltaTime : -Time.deltaTime) * fadeSpeed; //add time to 1

			bg_Music.volume = 1f - t;
			event_Music.volume = t;

			yield return null;
		}
		t = Mathf.Clamp(t, 0, 1);
		if(!done && resetEventMusicOnTransitionEnd) event_Music.Stop();
		transitioning = false;
	}
}



