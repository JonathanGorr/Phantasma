using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities : MonoBehaviour {

	public static Utilities Instance = null;

	void Awake()
	{
		if(Instance == null) Instance = this;
	}

	public void Hide(CanvasGroup cg)
	{
		StartCoroutine(CGFader(cg, false));
	}

	public void Reveal(CanvasGroup cg)
	{
		StartCoroutine(CGFader(cg, true));
	}

	IEnumerator CGFader(CanvasGroup cg, bool reveal)
	{
		//turn off interaction immediately if hiding
		if(!reveal)
		{
			cg.interactable = false;
			cg.blocksRaycasts = false;
		}
		//lerp
		float t = reveal ? 0 : 1;
		while((t < 1 && reveal) || (t > 0 && !reveal))
		{
			t += (reveal ? Time.deltaTime : -Time.deltaTime) * 4;
			cg.alpha = t;
			yield return null;
		}
		//turn on interaction at the end of the fade if revealing
		if(reveal)
		{
			cg.interactable = true;
			cg.blocksRaycasts = true;
		}
	}
}
