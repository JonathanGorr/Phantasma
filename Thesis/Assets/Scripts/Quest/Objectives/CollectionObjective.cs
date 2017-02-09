using UnityEngine;

namespace QuestSystem
{
	public class CollectionObjective : QuestObjective
	{
		/// <summary>
		/// Collection Objective:
		/// A quest objective where the player must collect x number of things
		/// </summary>

		[Header("Collection Objective")]
		public string verb = "";
		public int collectionAmount = 0; //the amount to be collected
		public int currentAmount = 0; //the current amount collected
		public Item itemToCollect; //replace with Item script perhaps

		public override void CheckProgress()
		{
			if(currentAmount >= collectionAmount)
			{
				Debug.Log("collection quest completed");
				isComplete = true;
			}
			else
			{
				isComplete = false;
			}
		}

		void Update()
		{
			CheckProgress();
		}

		public void UpdateProgress()
		{
			CheckProgress();
		}

		public override string Title
		{
			//i.e. Collect 5 bats.
			get { return Quest.Title + " : " + verb + " " + collectionAmount + " " + itemToCollect.Title; }
		}

		public override string Description
		{
			get { return currentAmount + "/" + collectionAmount + " " + itemToCollect.Title + " " + verb + "ed!"; }
		}

		public override string ToString()
		{
			//says 5/10 gathered!
			return Title;
		}
	}
}
