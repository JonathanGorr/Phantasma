using UnityEngine;

namespace QuestSystem 
{
	/// <summary>
	/// Location objective:
	/// This is a template for a location objective or an objective that is achieved simply moving to a location
	/// </summary>

	[System.Serializable]
	public class LocationObjective : QuestObjective
	{
		[Header("Location Objective")]
		public string noun; //who or what am I visiting?
		private string verb = "Visit";
		private Location _playerLocation; //where is my player?
		public Location _objectiveLocation; //where is the quest location?

		public Location ObjectiveLocation
		{
			get { return _objectiveLocation; }
			set { _objectiveLocation = value; }
		}

		public Location MyPlayersLocation
		{
			get { return _playerLocation; }
			set { _playerLocation = value; }
		}

		public override string Title 
		{
			//get the quest name via QuestManager lookup
			get { return Quest.Title + " : " + verb + " " + noun; }
		}

		public override string Description 
		{
			get { return verb + " the " + _objectiveLocation.Zone + "."; }
		}

		//must be called by update in a monobehaviour...
		//compares the desired location for this objective with stored player reference location
		public override void CheckProgress()
		{
		}

		public override string ToString()
		{
			return Description;
		}
	}
}
