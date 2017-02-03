using UnityEngine;
using System.Collections;

public class BossFight : MonoBehaviour {

	private BossHealthBar _bossHealthBar;
	private GameObject _victoryScreen;
	private MusicFader _mFader;

	void Awake()
	{
		_mFader = GameObject.Find ("Music").GetComponent<MusicFader>();
		_bossHealthBar = GameObject.Find("_LevelManager").transform.Find("UI/BossHealthBar").GetComponent<BossHealthBar>();
		_bossHealthBar.gameObject.SetActive (false);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "Player")
		{
			_mFader.FadeTo(_mFader.bossTheme);
			_bossHealthBar.gameObject.SetActive(true);
			_bossHealthBar.bossFight = true;
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if(other.tag == "Player")
		{
			_mFader.FadeTo(_mFader.forestTheme);
			_bossHealthBar.gameObject.SetActive(false);
			_bossHealthBar.bossFight = false;
		}
	}
}
