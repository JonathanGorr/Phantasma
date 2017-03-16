using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuestSystem;

/// <summary>
/// Quest collider:
/// Subscribes to specific quest.
/// Activates or deactivates on quest status update.
/// I.e. destroys colliders if quest activated or completed
/// </summary>

public class QuestCollider : MonoBehaviour {

	public Collider2D collider;
	public Objective objective;

	void Start()
	{
		//if this objective is inactive, don't update
		QuestObjective obj = QuestManager.Instance.GetQuestObjective(objective.questKey, objective.objectiveKey);
		if(obj) obj.onActivated += Disable;
	}

	public virtual void Destroy()
	{
		Destroy(this.gameObject);
	}

	public virtual void Disable()
	{
		StartCoroutine(DisableCollider());
	}

	public virtual IEnumerator DisableCollider()
	{
		#if UNITY_EDITOR
		print("disabled collider");
		#endif
		collider.enabled = false;
		yield return null;
	}

	public virtual void Enable()
	{
		StartCoroutine(EnableCollider());
	}

	public virtual IEnumerator EnableCollider()
	{
		yield return null;
	}
}
