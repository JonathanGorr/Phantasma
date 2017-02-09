using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuestSystem
{
	public class QuestTrigger : MonoBehaviour {

		private QuestManager _questManager;
		public Objective objective;

		void Awake()
		{
			_questManager = GameObject.Find("_LevelManager").GetComponent<QuestManager>();
		}

		void OnTriggerEnter2D(Collider2D col)
		{
			if(col.CompareTag("Player"))
			{
				//if this objective is inactive, don't update
				if(!_questManager.GetQuest(objective.questKey).IsActive) return;
				_questManager.GetQuestObjective(objective.questKey, objective.objectiveKey).SetComplete();
				//col.GetComponent<PlayerInfo>().CompleteObjective(objective);
			}
		}
	}
}
