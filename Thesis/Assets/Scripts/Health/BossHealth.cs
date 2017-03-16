using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealth : Health {

	MusicFader _mFader;

	public override void Start()
	{
		_mFader = GameObject.Find ("Music").GetComponent<MusicFader>();
		UpdateHealthBar();
	}

	public override void OnKill()
	{
		Utilities.Instance.Hide(GameObject.Find("BossHealthBar").GetComponent<CanvasGroup>());
		_mFader.FadeTo(_mFader.victoryTheme);
		base.OnKill();
	}
}
