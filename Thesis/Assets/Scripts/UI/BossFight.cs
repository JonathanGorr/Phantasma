using UnityEngine;
using System.Collections;

public class BossFight : MonoBehaviour {

	private GameObject _bossHealthBar;
	private GameObject _victoryScreen;
	private MusicFader _mFader;

	void Awake()
	{
		_mFader = GameObject.Find ("Music").GetComponent<MusicFader>();
		_bossHealthBar = GameObject.Find ("BossHealthBar");
		_bossHealthBar.SetActive (false);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "Player")
		{
			_mFader.Fade(_mFader.bossTheme);
			_bossHealthBar.SetActive(true);
			_bossHealthBar.GetComponent<BossHealthBar>().bossFight = true;
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if(other.tag == "Player")
		{
			_mFader.Fade(_mFader.forestTheme);
			_bossHealthBar.SetActive(false);
			_bossHealthBar.GetComponent<BossHealthBar>().bossFight = false;
		}
	}
}
