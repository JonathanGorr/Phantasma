using UnityEngine;
using System.Collections;

public class FadeOutSprite : MonoBehaviour {

	public bool fadeOnSpawn = true;
	public Renderer rend;
	bool fading = false;

	private float fadeTime = 3;
	public float FadeTime
	{
		set { fadeTime = value; }
	}

	public float fadeDelay = 3;
	public float Delay
	{
		set { fadeDelay = value; }
	}

	void OnEnable ()
	{
		if(!rend) rend = GetComponent<Renderer>();
		if(fadeOnSpawn) Fade();
	}

	public void Fade()
	{	
		if(fading) return;
		fading = true;
		StartCoroutine(FadeOut());
	}

	IEnumerator FadeOut()
	{
		yield return new WaitForSeconds(fadeDelay);
		float t = 0;
		while(t < fadeTime)
		{
			t += Time.deltaTime/fadeTime;
			rend.material.color = new Color(rend.material.color.r, rend.material.color.g, rend.material.color.b, Mathf.Lerp(1, 0, t));
			yield return null;
		}

		Destroy(this.gameObject);
	}
}
