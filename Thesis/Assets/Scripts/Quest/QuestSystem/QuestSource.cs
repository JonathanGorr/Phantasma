using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuestSystem;

/// <summary>
/// Quest test:
/// 1. Contains a quest or objective
/// 2. Passes this quest/objective to the player's quest list
/// 3. Then destroys or deactivates for this player
/// OR gives a hint after detecting that this player already has the quest
/// </summary>

//this is the quest and objective you are looking up to assign to the player
[System.Serializable]
public class Objective
{
	public string questKey = "";
	public string objectiveKey = "";
}

public class QuestSource : MonoBehaviour {

	//debug
	private Collider2D myCollider;
	public Objective objective;

	void Awake()
	{
		myCollider = GetComponent<Collider2D>();
	}

	//give a player a quest if trigger entered
	void OnTriggerEnter2D(Collider2D col)
	{
		if(col != null)
		{
			if(col.CompareTag("Player"))
			{
				StartCoroutine(AddQuest(col));
			}
		}
	}

	//adds quest after done talking
	IEnumerator AddQuest(Collider2D col)
	{
		while(ConversationManager.Instance.talking) yield return null;
		col.GetComponent<PlayerInfo>().AddObjective(this.objective);
		Physics2D.IgnoreCollision(col, myCollider);
	}
}
