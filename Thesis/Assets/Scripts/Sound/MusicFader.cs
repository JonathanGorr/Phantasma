using UnityEngine;
using System.Collections;

public class MusicFader : MonoBehaviour {

	private Transform player;

	private float fadeSpeed = 0f;
	public float transitionSpeed = 2f, safeDistance = 6f;
	
	private AudioClip track1, track2;

	public AudioClip
		menuTheme,
		forestTheme,
		battleTheme,
		motherTheme,
		fatherTheme,
		victoryTheme,
		bossTheme;

	void Awake()
	{
		if(Application.loadedLevelName == "Menu")
			track1 = menuTheme;
		else
			track1 = forestTheme;

		player = GameObject.FindGameObjectWithTag ("Player").transform;
		GetComponent<AudioSource>().clip = track1;
		GetComponent<AudioSource>().Play();
	}

	public void Fade(AudioClip music)
	{
		track2 = music;
		FadeOut ();
	}

	public void CheckIfSafe()
	{
		GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
		
		foreach(GameObject enemy in enemies)
		{
			float distance = Vector3.Distance(enemy.transform.position, player.position);

			/*
			if(distance < safeDistance)
				Fade(forestTheme);
			*/
		}
	}

	void FixedUpdate()
	{
		// Update the fade.
		if (fadeSpeed < 0)
		{
			GetComponent<AudioSource>().volume = Mathf.Max(GetComponent<AudioSource>().volume + fadeSpeed * Time.fixedDeltaTime * transitionSpeed, 0);
			// Done fading out.
			if(GetComponent<AudioSource>().volume == 0)
			{
				GetComponent<AudioSource>().clip = track2;
				GetComponent<AudioSource>().Play();
				FadeIn();
			}
		}
		else if (fadeSpeed > 0)
		{
			GetComponent<AudioSource>().volume = Mathf.Min(GetComponent<AudioSource>().volume + fadeSpeed * Time.deltaTime * transitionSpeed, 0.5f);
			// Done fading in.
			if(GetComponent<AudioSource>().volume == 0.5f)
				fadeSpeed = 0;
		}
	}
	
	void FadeIn(){
		if (GetComponent<AudioSource>().volume < 0.5f)
			fadeSpeed = 1;
	}
	
	void FadeOut(){
		if(GetComponent<AudioSource>().volume > 0)
			fadeSpeed = -1;
	}
}



