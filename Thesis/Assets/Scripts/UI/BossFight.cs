using UnityEngine;
using System.Collections;

public class BossFight : MonoBehaviour {

	private GameObject _victoryScreen;
	private MusicFader _mFader;
	public BossHealth bossHealth;
	public CanvasGroup bossHealthBar;

	void Awake()
	{
		_mFader = GameObject.Find ("Music").GetComponent<MusicFader>();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "Player")
		{
			_mFader.FadeTo(_mFader.bossTheme);
			Utilities.Instance.Reveal(bossHealthBar);
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if(other.tag == "Player")
		{
			_mFader.FadeTo(_mFader.forestTheme);
			Utilities.Instance.Hide(bossHealthBar);
		}
	}
}
