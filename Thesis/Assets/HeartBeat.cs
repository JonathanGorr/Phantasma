using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartBeat : MonoBehaviour {

	public AudioClip clip;
	public Animator anim;
	public AudioSource asrc;
	public float maxVolume = .5f;

	//set the animation speed, volume and sound speed to ratio of health to maxhealth
	public void Set(float speed)
	{
		anim.SetFloat("HealthRatio", speed);
		asrc.volume = Mathf.Lerp(0, maxVolume, speed);
		asrc.pitch = Mathf.Lerp(1, 2, speed);
	}

	public void Beat()
	{
		if(asrc.volume == 0) return;
		asrc.PlayOneShot(clip);
	}
}
