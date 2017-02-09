using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace QuestSystem
{
	public class QuestManager : MonoBehaviour {

		[HideInInspector] public UI _ui;
		[HideInInspector] public SFX _sfx;

		//A entry for the hack dictionary
		public List<Quest> questList = new List<Quest>();

		void Awake()
		{
			SceneManager.sceneLoaded += OnSceneLoaded;
			_ui = transform.Find("UI").GetComponent<UI>();
			_sfx = GetComponent<SFX>();
		}

		void OnSceneLoaded( Scene scene, LoadSceneMode m)
		{
			if(scene.name == "Start")
			{
				Clone();
			}
		}

		//runs the checkprogress() methods on each objective and quest
		public void UpdateProgress()
		{
			if(questList.Count == 0) return;

			//do for all quests
			for(int i=0;i<questList.Count;i++)
			{
				//early out if not active quest
				if(!questList[i].IsActive) continue;

				//check if finished
				questList[i].CheckProgress();

				//do for all objectives in this quest
				for(int j=0;j<questList[i].objectiveList.Count;j++)
				{
					//early out for inactive objectives
					if(!questList[i].objectiveList[j].IsActive) continue;

					//check if finished
					questList[i].objectiveList[j].CheckProgress();
				}
			}
		}

		//used to find quest in dictionary
		public Quest GetQuest(string questKey)
		{
			if(questList.Count == 0) { print("There are no quests in list."); return null; }
			for(int i=0;i<questList.Count;i++)
			{
				if(questList[i].Key == questKey) return questList[i];
			}
			print("This quest doesn't exist!");
			return null;
		}

		//used to find objective in particular quest
		public QuestObjective GetQuestObjective(string questKey, string objectiveKey)
		{
			Quest q = GetQuest(questKey);
			if(q.objectiveList.Count == 0) { print("This quest has no objectives."); return null; }

			return q.GetObjective(objectiveKey);

			print("Objective doesn't exist in quest list!");
			return null;
		}

		//replace all quests and their objectives with instances with loaded values
		void Clone()
		{
			if(questList.Count == 0) { print("no quests at all."); return; }

			for(int i=0;i<questList.Count;i++)
			{
				if(questList[i] == null) { print("This quest is null, please remove!"); continue; }

				//create an instance of every quest( as is a reference of mutable assets )
				questList[i] = ScriptableObject.Instantiate(questList[i]) as Quest;
				questList[i].QuestManager = this; //assign quest manager reference

				if(questList[i].objectiveList.Count == 0) { print("no objectives at all."); continue; } //continue with loop; but don't execute below

				//do for each objective
				for(int j=0;j<questList[i].objectiveList.Count;j++)
				{
					if(questList[i].objectiveList[j] == null) { print("This objective is null, please remove!"); continue; }

					//create an instance of each objective ( as they are mutable references to assets )
					questList[i].objectiveList[j] = ScriptableObject.Instantiate(questList[i].objectiveList[j]) as QuestObjective;
					questList[i].objectiveList[j].Quest = questList[i]; //assign quest
				}
			}
		}
	}
}
