using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuestSystem
{
	/// <summary>
	/// Quest trigger:
	/// gives the player quests/quest objectives on trigger enter
	/// Gets destroyed if the quest in questmanager is completed
	/// </summary>

	public class QuestTrigger : MonoBehaviour {

		public Objective objective;

		public enum CompleteAction 
		{ 
			destroy, //if the object is a sprite that you want to disappear
			ignore //if the object is an npc or something that doesn't need to be destroyed
		}

		public CompleteAction completedAction = CompleteAction.destroy;

		void Start()
		{
			//if this game has been loaded, and the quest is already completed, destroy, ignore or change dialog
			if(QuestManager.Instance.GetQuestObjective(objective.questKey, objective.objectiveKey).isComplete)
			{
				switch(completedAction)
				{
				case CompleteAction.destroy:
				Destroy(this.gameObject);
				break;
				case CompleteAction.ignore:
				GetComponentInChildren<Collider2D>().enabled = false;
				break;
				}
			}
		}

		void OnTriggerEnter2D(Collider2D col)
		{
			if(col.CompareTag("Player"))
			{
				//if this objective is inactive, don't update
				if(!QuestManager.Instance.GetQuest(objective.questKey).IsActive) return;
				//QuestManager.Instance.GetQuestObjective(objective.questKey, objective.objectiveKey).SetComplete();
			}
		}
	}
}
