using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class PostProcessingHandler : MonoBehaviour {

	public static PostProcessingHandler Instance = null;

	//vignette
	public VignetteAndChromaticAberration vignette;
	private float vignetting;
	public float lerpSpeed = 4;

	void Awake()
	{
		if(Instance == null) Instance = this;
		PauseMenu.pause += ShowVignette;
		PauseMenu.unPause += HideVignette;

		vignetting = vignette.intensity;
	}

	void OnDisable()
	{
		PauseMenu.pause -= ShowVignette;
		PauseMenu.unPause -= HideVignette;
	}

	public void Pause()
	{
		ShowVignette();
	}
	void UnPause()
	{
		HideVignette();
	}

	public void ShowVignette()
	{
		StartCoroutine(Vignette(true));
	}
	public void HideVignette()
	{
		StartCoroutine(Vignette(false));
	}

	IEnumerator Vignette(bool reveal)
	{
		if(reveal) vignette.enabled = true;
		//lerp
		float t = reveal ? 0 : 1;
		while((t < 1 && reveal) || (t > 0 && !reveal))
		{
			t += (reveal ? Time.deltaTime : -Time.deltaTime) * lerpSpeed;
			vignette.intensity = Mathf.Lerp(0, vignetting, t);
			yield return null;
		}
		if(!reveal) vignette.enabled = false;
	}
}
