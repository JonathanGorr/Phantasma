using UnityEngine;
using System.Collections;

public class MusicFader : MonoBehaviour {

	private float fadeSpeed = 0f;

	public float transitionSpeed = 2f;
	
	private AudioClip track1, track2;

	public AudioClip
		forestTheme,
		combatTheme,
		motherTheme,
		fatherTheme;

	void Awake()
	{
		track1 = forestTheme;
		audio.clip = track1;
		audio.Play();
	}

	public void Fade(AudioClip music)
	{
		track2 = music;
		FadeOut ();
		//print ("fading");
	}

	void FixedUpdate()
	{
		// Update the fade.
		if (fadeSpeed < 0)
		{
			audio.volume = Mathf.Max(audio.volume + fadeSpeed * Time.fixedDeltaTime * transitionSpeed, 0);
			// Done fading out.
			if(audio.volume == 0)
			{
				audio.clip = track2;
				audio.Play();
				FadeIn();
			}
		}
		else if (fadeSpeed > 0)
		{
			audio.volume = Mathf.Min(audio.volume + fadeSpeed * Time.deltaTime * transitionSpeed, 0.5f);
			// Done fading in.
			if(audio.volume == 0.5f)
			{
				fadeSpeed = 0;
			}
		}
	}
	
	void FadeIn(){
		if (audio.volume < 0.5f){
			fadeSpeed = 1;
		}
	}
	
	void FadeOut(){
		if(audio.volume > 0)
		{
			fadeSpeed = -1;
		}
	}
}



