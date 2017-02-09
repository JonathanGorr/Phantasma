using UnityEngine;

namespace QuestSystem
{
	public class AssassinateObjective : QuestObjective
	{
		/// <summary>
		/// Collection Objective:
		/// A quest objective where the player must collect x number of things
		/// </summary>

		[Header("Assassination Objective")]
		public string verb = "";
		public int killTotal = 0; //the amount to be collected
		public int currentKills = 0; //the current amount collected
		public Enemy enemyToKill; //replace with Item script perhaps

		public override void CheckProgress()
		{
			if(currentKills >= killTotal)
			{
				Debug.Log("assassination quest completed");
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
			get 
			{ 
				if(enemyToKill)
				{
					return verb + " " + killTotal + " " + enemyToKill.title; 
				}
				else
				{
					return verb + " " + killTotal + " " + (killTotal > 1 ? "enemies." : "enemy."); 
				}
			}
		}

		public override string Description
		{
			get 
			{
				if(enemyToKill)
				{ 
					return currentKills + "/" + killTotal + " " + enemyToKill.title + " " + verb + "ed!";
				}
				else
				{
					return currentKills + "/" + killTotal + " " + "enemies" + " " + verb + "ed!";
				}
			}
		}

		public override string ToString()
		{
			//says 5/10 bats killed!
			return Title;
		}
	}
}
