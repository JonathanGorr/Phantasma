using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Swinging door quest collider:
/// A type of quest collider that is a swinging door
/// </summary>

public class SwingingDoorQuestCollider : QuestCollider {

	public Animator anim;

	public override IEnumerator DisableCollider()
	{	
		collider.enabled = false;

		anim.SetTrigger("Open");
		SFX.Instance.PlayFX("door", transform.position);

		CameraController.Instance.UnRegisterMe(Player.Instance.transform);
		CameraController.Instance.RegisterMe(transform);

		yield return new WaitForSeconds(1.5f);

		CameraController.Instance.UnRegisterMe(transform);
		CameraController.Instance.RegisterMe(Player.Instance.transform);
	}
}
