using UnityEngine;
using System.Collections.Generic;
using QuestSystem;

/// <summary>
/// PlayerInfo:
/// This class is used to interact with the quest and multiplayer system
/// It contains information like:
/// 1. player location( zone and v2 )
/// 2. Id for lookup
/// 3. Quests associated with this character
/// </summary>

public class PlayerInfo : MonoBehaviour {

	private QuestSystem.QuestManager _questManager;
	private UI _ui;
	private SFX _sfx;

	[Header("Costume Colors")]
	public SpriteRenderer[] torso;
	public SpriteRenderer[] pants;
	public SpriteRenderer[] shoes;
	public SpriteRenderer[] armor;

	[Header("Location")]
	private Location location = new Location(Location.ZoneType.House);
	public Location Location
	{
		get { return location; }
	}

	void Start()
	{
		_sfx = GameObject.Find("_LevelManager").GetComponent<SFX>();
		_questManager = _sfx.GetComponent<QuestManager>();
		_ui = _sfx.transform.Find("UI").GetComponent<UI>();
	}

	//used to add new objective to a player's quest( objective should not(?) give player the associated quest...
	public void AddObjective(Objective obj)
	{
		//check if objective is active before adding
		if(_questManager.GetQuest(obj.questKey).IsActive) { print("quest already active for this player"); return; }

		//if this is a location objective...
		if(_questManager.GetQuestObjective(obj.questKey, obj.objectiveKey).GetType() == typeof(LocationObjective))
		{
			//give it a reference to the player's location for comparison
			LocationObjective lo = _questManager.GetQuestObjective(obj.questKey, obj.objectiveKey) as LocationObjective;
			lo.MyPlayersLocation = location;
		}

		_questManager.GetQuest(obj.questKey).Activate();
		_questManager.GetQuestObjective(obj.questKey, obj.objectiveKey).Activate();
	}

	public void UpdateObjective(Objective obj)
	{
		//check if objective is active before adding
		if(!_questManager.GetQuest(obj.questKey).IsActive) { print("quest inactive for this player"); return; }

		_questManager.GetQuestObjective(obj.questKey, obj.objectiveKey).SetComplete();
	}
}
