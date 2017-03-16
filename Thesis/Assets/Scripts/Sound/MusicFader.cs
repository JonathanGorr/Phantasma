using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// Music fader.
/// bg_music Plays a constant background song
/// event_music fades in a different dialog or event-specific, temporary theme to be later faded out
/// </summary>

public class MusicFader : MonoBehaviour {

	public static MusicFader Instance = null;

	[Tooltip("Press J to start transition")] public bool debug;

	[Header("Audio Sources")]
	public AudioSource bg_Music;
	public AudioSource event_Music;

	[Header("Settings")]
	public float speed = 1;
	bool eventOn = false;
	bool transitioning = false;
	float t = 0;
	float volume = 0.5f;

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
		if(Instance == null) Instance = this;
		SceneManager.sceneLoaded += OnSceneLoaded;

		if(PlayerPrefs.GetInt("GameCreated") == 0)
		{
			bg_Music.volume = volume;
			PlayerPrefs.SetFloat("MusicVolume", volume);
		}
		else if(PlayerPrefs.GetInt("GameCreated") == 1)
		{
			bg_Music.volume = PlayerPrefs.GetFloat("MusicVolume");
		}
	}

	#if UNITY_EDITOR
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
	#endif

	void OnApplicationQuit()
	{
		PlayerPrefs.SetFloat("MusicVolume", volume);
		PlayerPrefs.Save();
	}

	public void SetVolume(float v)
	{
		//set volume of whichever one is currently unmuted/playing
		volume = v;
		if(!bg_Music.mute) bg_Music.volume = v;
		if(!event_Music.mute) event_Music.volume = v;
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
		eventOn = !eventOn; //toggle

		//unmute these immediately
		if(eventOn)
			event_Music.mute = false;
		else
			bg_Music.mute = false;
		
		while((eventOn && t < volume) || (!eventOn && t > 0)) //while not 1...
		{
			t += (eventOn ? Time.deltaTime : -Time.deltaTime) * speed; //add time to 1

			bg_Music.volume = volume - t;
			event_Music.volume = t;

			yield return null;
		}
		t = Mathf.Clamp(t, 0, volume);

		//mute these finally
		if(eventOn)
			bg_Music.mute = true;
		else
			event_Music.mute = true;

		transitioning = false;
	}
}



