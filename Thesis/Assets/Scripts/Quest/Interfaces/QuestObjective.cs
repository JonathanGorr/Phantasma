using UnityEngine;

namespace QuestSystem
{
	[System.Serializable]
	public class QuestObjective : ScriptableObject, IQuestObjective
	{
		[Header("Quest Objective")]
		private Quest quest;
		private ObjectiveUI objUI; //a reference to it's UI for updating
		public string key; //keeps a copy of it's own key for reference to place in dictionary
		private string title;
		private string description;
		public bool isActive;
		public bool isComplete;
		public bool isBonus;
		public Item[] rewards;

		public virtual Quest Quest
		{
			get { return quest; }
			set { quest = value; }
		}

		public virtual ObjectiveUI ObjectiveUI
		{
			get { return objUI; }
			set { objUI = value; }
		}

		public virtual string Key
		{
			get { return key; }
			set { key = value; }
		}

		//used to return title
		public virtual string Title
		{
			get { return title; }
		}

		public virtual bool IsActive
		{
			get { return isActive; }
		}

		public virtual void Activate()
		{
			//gets a new objectiveUI if not already existing
			if(!objUI) objUI = quest.QuestManager._ui.GetObjectiveUI();
			quest.QuestManager._sfx.PlayUI("questUpdate");
			quest.QuestManager._ui.QuestUpdate(1);
			isActive = true;
			UpdateUI();
		}

		public virtual void Deactivate()
		{
			isActive = false;
		}

		public virtual bool IsComplete
		{
			get { return isComplete; }
		}

		public void SetComplete()
		{
			isComplete = true;
			quest.QuestManager._sfx.PlayUI("questCompleted");
			quest.QuestManager._ui.QuestUpdate(1);
			UpdateUI();
		}

		void UpdateUI()
		{
			objUI.UpdateUI(this);
		}

		public void Fail()
		{
			isComplete = false;
			isActive = false;
			Debug.Log("quest failed");
		}

		public virtual bool IsBonus
		{
			get { return isBonus; }
		}

		//used to return description
		public virtual string Description
		{
			get { return description; }
		}

		public virtual void CheckProgress()
		{
			
		}

		public override string ToString()
		{
			return title + ".";
		}
	}
}
