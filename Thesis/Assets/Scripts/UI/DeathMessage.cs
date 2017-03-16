using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DeathMessage : WaitForPlayer {

	public CanvasGroup deathMessage;
	public float delay = 3f;

	public override IEnumerator Initialize(UnityEngine.SceneManagement.Scene scene)
	{
		while(Player.Instance == null) yield return null;

		Player.Instance._health.onDeath += StartRestart;
	}

	public void StartRestart()
	{
		StartCoroutine ("Restart");
	}

	public IEnumerator Restart()
	{
		//CameraController.Instance.enabled = false;
		Utilities.Instance.Reveal(deathMessage);
		yield return new WaitForSeconds(delay);
		Utilities.Instance.Hide(deathMessage);
		LevelManager.Instance.StartRestart ();
	}
}
