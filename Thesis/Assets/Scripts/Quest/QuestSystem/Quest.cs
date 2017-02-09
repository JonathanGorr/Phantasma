using System.Collections.Generic;
using UnityEngine;

namespace QuestSystem 
{
	[System.Serializable]
	public class Quest : ScriptableObject, IQuest
	{
		QuestManager _qm;
		public string key;
		public string title;
		public string description;
		public string hint;
		public bool isActive;
		public bool isComplete;
		public Item reward;

		public List<QuestObjective> objectiveList = new List<QuestObjective>();

		public QuestManager QuestManager
		{
			get { return _qm; }
			set { _qm = value; }
		}

		public virtual string Key
		{
			get { return key; }
			set { key = value; }
		}

		public virtual string Title
		{
			get { return title; }
		}

		public string Hint
		{
			get { return hint; }
		}

		void Update()
		{
			Debug.Log("checking");
		}

		public int ObjectiveCount
		{
			get { return objectiveList.Count; }
		}

		public QuestObjective GetObjective(string objKey)
		{
			for(int i=0;i<objectiveList.Count;i++)
			{
				if(objectiveList[i].Key == objKey) return objectiveList[i];
			}
			return null;
		}

		public bool HasObjective(string objKey)
		{
			for(int i = 0; i< objectiveList.Count;i++)
			{
				if(objectiveList[i].Key == objKey)
				{
				 	return true;
				}
			}
			return false;
		}

		public virtual string Description
		{
			get { return description; }
		}

		public virtual bool IsComplete
		{
			get { return CheckProgress(); }
		}

		public void SetComplete()
		{
			isComplete = true;
			Debug.Log("objective complete");
		}

		public void Fail()
		{
			isComplete = false;
			isActive = false;
			Debug.Log("objective failed");
		}

		public virtual void Activate()
		{
			isActive = true;
		}

		public virtual void Deactivate()
		{
			isActive = false;
		}

		public virtual bool IsActive
		{
			get { return isActive; }
		}

		//check if all objectives are complete
		public virtual bool CheckProgress()
		{
			for (int i = 0; i < objectiveList.Count; i++) 
			{
				//check if any of this quest's objectives are incomplete and not bonuses
				if(objectiveList[i].isComplete && !objectiveList[i].isBonus) 
				{
					return false;
				}
			}

			//all objectives complete; get reward
			return true;
		}
	}
}
