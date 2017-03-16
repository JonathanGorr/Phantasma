using UnityEngine;
using System.Collections.Generic;
using System.Collections;
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

	public CharacterController.CharacterController2D _controller;

	GameObject drop; //the item currently stood over

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

		_controller.onTriggerEnterEvent += OnTriggerEnterHandler;
		_controller.onTriggerExitEvent += OnTriggerExitHandler;

		//use a to try and pickup a stood over item
		PlayerInput.onA += StartTryPickup;
	}

	//test if the drop list is empty
	bool DropEmpty(int[] dropList)
	{
		for(int i=0; i<dropList.Length;i++)
		{
			if(dropList[i] != -1) return false;
		}

		return true;
	}

	void StartTryPickup()
	{
		if(drop == null) return;
		StartCoroutine("TryPickup");
	}

	IEnumerator TryPickup()
	{
		Pickup pickup = drop.GetComponent<Pickup>();
		List<int> pickedUpThings = new List<int>();

		//try to pick up all items on drop
		for(int i = 0; i < pickup.items.Length; i++)
		{
			if(pickup.items[i] == -1) continue;//skip this; it's empty

			//check on each added item if the inventory is full
			if(AddItem(pickup.items[i]))
			{
				pickedUpThings.Add(pickup.items[i]); //add things that i've actually picked up to this list
				//set the item slot to "empty" or -1
				pickup.items[i] = -1;
			}
			else continue;
		}

		UIPickupHandler.Instance.Pickup(pickedUpThings);

		//if the loot drop no longer contains any items, destroy
		if(DropEmpty(pickup.items))	
		{
			Destroy(drop);
			drop = null;
			UIPickupHandler.Instance.HidePickup();
		}

		SFX.Instance.PlayFX("item_pickup", transform.position);

		//wait until a depressed until one can jump again
		while(PlayerInput.Instance.ADown) { yield return null; }
		Player.Instance.canJump = true;
	}

	void OnTriggerEnterHandler(Collider2D col)
	{
		if(col.CompareTag("Pickup"))
		{
			drop = col.gameObject;
			UIPickupHandler.Instance.RevealPickup();
			Player.Instance.canJump = false;
		}
	}

	void OnTriggerExitHandler(Collider2D col)
	{
		if(col.CompareTag("Pickup"))
		{
			print("trigger exited");
			drop = null;
			UIPickupHandler.Instance.HidePickup();
			Player.Instance.canJump = true;
		}
	}

	public bool AddItem(int id)
	{
		//get item
		Item item = Inventory.Inventory.Instance.database.FetchItem(id);

		if(item.slots) //these items take up slots in inventory
		{
			if(Inventory.Inventory.Instance.AddItem(item.id, true))
				return true;
			else
				return false;
		}
		else //these items don't take up space in the inventory
		{
			switch(item.slug)
			{
				case "coin":
				Inventory.Inventory.Instance.AddCoins(item.value);
				return true;
				case "blood":
				Evolution.Instance.AddBlood(item.value);
				return true;
			}
		}

		SFX.Instance.PlayFX(item.slug + "_pickup", transform.position);
		return false;
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
