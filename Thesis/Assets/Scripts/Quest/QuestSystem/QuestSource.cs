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
	private QuestManager _questManager;
	/*public*/ Quest questRef;
	/*public*/ QuestObjective objectiveRef;
	private Collider2D myCollider;
	public Objective objective;

	void Awake()
	{
		myCollider = GetComponent<Collider2D>();
		_questManager = GameObject.Find("_LevelManager").GetComponent<QuestManager>();
		this.questRef = _questManager.GetQuest(objective.questKey);
		this.objectiveRef = _questManager.GetQuestObjective(objective.questKey, objective.objectiveKey);
	}

	//give a player a quest if trigger entered
	void OnTriggerEnter2D(Collider2D col)
	{
		if(col != null)
		{
			if(col.CompareTag("Player"))
			{
				//print("called");
				col.GetComponent<PlayerInfo>().AddObjective(this.objective);
				Physics2D.IgnoreCollision(col, myCollider);
			}
		}
	}
}
