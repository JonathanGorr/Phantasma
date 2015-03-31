using UnityEngine;
using System.Collections;

public class BossFight : MonoBehaviour {

	private GameObject _bossHealthBar;
	private GameObject _victoryScreen;

	void Awake()
	{
		_bossHealthBar = GameObject.Find ("BossHealthBar");
		_bossHealthBar.SetActive (false);
	}

	void OnTriggerEnter2D(Collider2D player)
	{
		if(player.gameObject.tag == "Player")
		{
			_bossHealthBar.SetActive(true);
			_bossHealthBar.GetComponent<BossHealthBar>().bossFight = true;
		}
	}
}
