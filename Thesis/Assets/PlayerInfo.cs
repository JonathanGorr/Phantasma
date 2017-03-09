using UnityEngine;
using System.Collections.Generic;
using QuestSystem;
using Inventory;

/// <summary>
/// PlayerInfo:
/// This class is used to interact with the quest and multiplayer system and inventory system
/// It contains information like:
/// 1. player location( zone and v2 )
/// 2. Id for lookup
/// 3. Quests associated with this character
/// </summary>

public class PlayerInfo : MonoBehaviour {

	public static PlayerInfo Instance = null;

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

	void Awake()
	{
		if(Instance == null) Instance = this;
	}

	public void AddItem(int id)
	{
		//get item
		Item item = Inventory.Inventory.Instance.database.FetchItem(id);

		if(item.slots) //these items take up slots in inventory
		{
			Inventory.Inventory.Instance.AddItem(item.id, true);
		}
		else //these items don't take up space in the inventory
		{
			switch(item.slug)
			{
				case "coin":
				Inventory.Inventory.Instance.AddCoins(item.value);
				break;
				case "blood":
				Evolution.Instance.AddBlood(item.value);
				break;
			}
		}

		SFX.Instance.PlayFX(item.slug + "_pickup", transform.position);

	}

	//used to add new objective to a player's quest( objective should not(?) give player the associated quest...
	public void AddObjective(Objective obj)
	{
		//check if objective is active before adding
		if(QuestManager.Instance.GetQuest(obj.questKey).IsActive) { print("quest already active for this player"); return; }

		//if this is a location objective...
		if(QuestManager.Instance.GetQuestObjective(obj.questKey, obj.objectiveKey).GetType() == typeof(LocationObjective))
		{
			//give it a reference to the player's location for comparison
			LocationObjective lo = QuestManager.Instance.GetQuestObjective(obj.questKey, obj.objectiveKey) as LocationObjective;
			lo.MyPlayersLocation = location;
		}

		QuestManager.Instance.GetQuest(obj.questKey).Activate();
		QuestManager.Instance.GetQuestObjective(obj.questKey, obj.objectiveKey).Activate();
	}

	public void UpdateObjective(Objective obj)
	{
		//check if objective is active before adding
		if(!QuestManager.Instance.GetQuest(obj.questKey).IsActive) { print("quest inactive for this player"); return; }
		QuestManager.Instance.GetQuestObjective(obj.questKey, obj.objectiveKey).SetComplete();
	}
}
