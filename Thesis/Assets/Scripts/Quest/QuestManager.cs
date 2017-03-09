using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

namespace QuestSystem
{
	//JSON STUFF
	[System.Serializable]
	public class QuestArray
	{
		public List<QuestEntry> quests; //IMPORTANT: this must be named the same as the json array string
	}

	[System.Serializable]
	public class QuestEntry
	{
		public int ID;
		public string Slug;
		public string Title;
		public string Description;
		public string Hint;
		public bool IsActive;
		public bool IsComplete;
		public int[] rewards;
		public List<ObjectiveEntry> objectives;
	}

	[System.Serializable]
	public class ObjectiveEntry
	{
		public int ID;
		public string Slug;
		public string Title;
		public string Description;
		public string Hint;
		public bool IsActive;
		public bool IsComplete;
		public int[] rewards;
	}

	public class QuestManager : MonoBehaviour 
	{
		public static QuestManager Instance = null;

		//A entry for the hack dictionary
		public List<Quest> questList = new List<Quest>();

		public QuestArray questDatabase;

		void Awake()
		{
			if(Instance == null) Instance = this;
			SceneManager.sceneLoaded += OnSceneLoaded;
			CreateNewDatabase();
		}

		void OnSceneLoaded( Scene scene, LoadSceneMode m)
		{
			if(scene.name == "Start")
			{
				Clone();
			}
		}

		//copies streaming assets
		public string CreateNewDatabase() //TODO: you can make these methods generic and static
		{
			//fill database with inventory.json
			JsonUtility.FromJsonOverwrite(StaticMethods.GetStreamingAsset("Quests.json"), questDatabase);

			//TODO: construct the quests UI menu from here

			#if UNITY_EDITOR
			//print("created quest database");
			#endif
			return JsonUtility.ToJson(questDatabase);
		}

		//returns the current database for saving
		public string GetMyDatabase()
		{
			#if UNITY_EDITOR
			print("saving quest database");
			#endif
			return JsonUtility.ToJson(questDatabase);
		}

		//creates database from save game
		public void LoadDatabase()
		{
			JsonUtility.FromJsonOverwrite(SaveSystem.thisSaveGame.inventory, questDatabase);

			//create the inventory after database created
			//Inventory.Instance.ConstructInventory();
			#if UNITY_EDITOR
			print("loaded quest database");
			#endif
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
			if(q == null) print("Objective doesn't exist in quest list!");
			return q.GetObjective(objectiveKey);
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
