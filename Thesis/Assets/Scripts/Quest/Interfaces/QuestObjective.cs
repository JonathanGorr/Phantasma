using UnityEngine;
using Inventory;

namespace QuestSystem
{
	[System.Serializable]
	public class QuestObjective : ScriptableObject, IQuestObjective
	{
		ItemDatabase _dataBase;
		[Header("Quest Objective")]
		private Quest quest;
		private ObjectiveUI objUI; //a reference to it's UI for updating
		public string key; //keeps a copy of it's own key for reference to place in dictionary
		private string title;
		private string description;
		public bool isActive;
		public bool isComplete;
		public bool isBonus;
		public string[] rewards;

		public delegate void OnComplete();
		public event OnComplete onCompleted;

		public delegate void OnActivated();
		public event OnActivated onActivated;

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

		public virtual ItemDatabase DataBase
		{
			get { return _dataBase; }
			set { _dataBase = value; }
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
			if(onActivated!=null) onActivated();
			//gets a new objectiveUI if not already existing
			if(!objUI) objUI = UI.Instance.GetObjectiveUI();
			SFX.Instance.PlayUI("questUpdate");
			UI.Instance.QuestUpdate(1);
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
			if(onCompleted!=null) onCompleted();
			isComplete = true;
			SFX.Instance.PlayUI("questCompleted");
			UI.Instance.QuestUpdate(1);
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
