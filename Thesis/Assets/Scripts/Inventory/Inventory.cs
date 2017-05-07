using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

namespace Inventory
{
	/// <summary>
	/// Responsible for interfacing with the UI of the inventory as well as adding/removing items from the database.
	/// </summary>
	public class Inventory : MonoBehaviour 
	{
		public static Inventory Instance = null; 	//simple singleton

		public InventoryInfo _info;					//The panel used to display information about a selected item
		public RectTransform inventoryPanel;		//our inventory ui panel
		public ItemDatabase database;				//our item database
		public RectTransform itemPrefab;			//our ui item prefab
		public RectTransform slotPrefab;			//our slot ui prefab
		public Text coinText;						//coin text

		//how many coins this inventory contains.
		int coins;
		/// <summary>
		/// Public coin get/setter.
		/// </summary>
		/// <value>The coins.</value>
		public int Coins
		{
			get { return coins; }
			set { coins = value; }
		}

		/// <summary>
		/// Adds coins to the player's inventory.
		/// </summary>
		/// <param name="count">Count.</param>
		public void AddCoins(int count)
		{
			coins += count;
			UpdateCoinUI();
		}

		/// <summary>
		/// Updates the inventories coin count text. //TODO: may be better to handle this in different UI-focused script
		/// </summary>
		public void UpdateCoinUI()
		{
			coinText.text = coins.ToString();
		}

		int slotAmount = 20;
		[HideInInspector] public List<RectTransform> slots = new List<RectTransform>();
		/*[HideInInspector]*/ public List<Item> items = new List<Item>();

		void Awake()
		{
			if(Instance == null) Instance = this;
			LevelManager.onGoToMenu += Clear;
		}

		//destroys everything in inventory( called by quit game ).
		void Clear()
		{
			//destroy all slots
			for(int i=0;i<slots.Count;i++)
			{
				Destroy(slots[i].gameObject);
			}

			items.Clear();
			slots.Clear();
		}

		/// <summary>
		/// Used to create UI slots for our inventory.
		/// </summary>
		public void ConstructInventory()
		{
			//create UI slots for inventory
			if(slots.Count == 0)
			{
				//add slots
				for(int i=0; i<slotAmount;i++)
				{
					//add an empty placeholder item to our item list
					items.Add(new Item());
					//instantiate the UI slot prefab
					RectTransform rt = Instantiate(slotPrefab, inventoryPanel);
					//reset local Scale ( this remedies scale problems symptomatic of transform parenting ).
					rt.localScale = Vector3.one;
					//name slot slot1, slot2, etc
					rt.name = "Slot " + i;
					//add slot to slots list
					slots.Add(rt);
					//grab itemslot component
					ItemSlot slot = slots[i].GetComponent<ItemSlot>();
					//set it's id to i
					slot.ID = i;
					//give it our reference
					slot.Inventory = this;
				}
			}

			//LOADING INVENTORY ITEMS FROM JSON INTO INVENTORY SLOTS
			//create an inventory slot for each item with an amount > 0)
			//set data.amount to inventoryEntry.count
			for(int i=0;i<ItemDatabase.Instance.database.items.Count;i++)
			{
				//skip items in database with 0 count
				if(ItemDatabase.Instance.database.items[i].count == 0) continue;
				//spawn count of each item
				for(int j=0; j<ItemDatabase.Instance.database.items[i].count;j++)
				{
					//add 1 item by id for each
					AddItem(i, false);
				}
			}
		}

		#if UNITY_EDITOR
		/// <summary>
		/// Used to test the adding of items to the inventory.
		/// </summary>
		void Update()
		{
			if(Input.GetKeyDown(KeyCode.K))
				AddItem(6, true);
			if(Input.GetKeyDown(KeyCode.L))
				AddItem(5, true);
		}
		#endif

		/// <summary>
		/// Attempts to add an item into our inventory.
		/// If we have too many of one item, or our inventory is full, cannot add item.
		/// </summary>
		/// <returns><c>true</c>, if item was added, <c>false</c> otherwise.</returns>
		/// <param name="id">Item ID.</param>
		/// <param name="addToDatabase">If set to <c>true</c> add to database.</param>
		public bool AddItem(int id, bool addToDatabase)
		{
			//get the item we are attempting to pass from our database
			Item itemToAdd = ItemDatabase.Instance.FetchItem(id);
			//does the item slot into the inventory UI? coins don't slot; potions do slot.
			if(itemToAdd.slots)
			{
				if(itemToAdd.stackable //is the item stackable; or does it consume a whole inventory slot?
				&& IsInInventory(itemToAdd)) //stack if present already
				{
					//for all items in inventory
					for(int i=0; i<items.Count; i++) 
					{
						//if we found the item slot we're stacking into...
						if(items[i].id == itemToAdd.id)
						{
							//get the item data component from the associated slot
							ItemData data = slots[i].transform.GetChild(0).GetComponent<ItemData>();
							//if(data.stackSize < items[i].stackLimit) //TODO: add support for per-item stack limits
							//{
								if(addToDatabase) itemToAdd.count ++;
								data.amount = itemToAdd.count;
								data.UpdateCount();
								return true;
							//}
						}
					}
				}
				//the item is not stackable or isn't already in our inventory
				else
				{
					//add items to slots
					for(int i=0; i<items.Count; i++)
					{
						//empty UI inventory slots all have a slot id of -1; if we found an empty slot
						if(items[i].id == -1)
						{
							//set inventory item array slot to our added item
							items[i] = itemToAdd;
							//create an inventory UI slot
							RectTransform itemObj = Instantiate(itemPrefab, slots[i].transform);
							//zero out position and scale
							RectTransform rt = itemObj.GetComponent<RectTransform>();
							rt.localPosition = Vector2.zero;
							rt.sizeDelta = Vector2.zero;
							//set sprite to associated sprite in resources
							itemObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Items/" + itemToAdd.slug);
							//set our item data to our item UI object's item data
							ItemData data = itemObj.GetComponent<ItemData>();
							//slot = item[this]
							data.slot = i;
							//increment the amount of this item
							data.amount ++;
							//set item to added
							data.Item = itemToAdd;
							//update the count ui by the method in our data component
							data.UpdateCount();
							//
							if(addToDatabase) itemToAdd.count ++;
							return true;
						}
					}
					//we found no empty slots
					SFX.Instance.PlayUI("error");
					Debug.Log("inventory full");
					return false;
				}
			}
			//update database with item amount?
			if(addToDatabase) itemToAdd.count ++;
			//return true; item can be added
			return true;
		}

		/// <summary>
		/// Determines whether an item is in our inventory.
		/// </summary>
		/// <returns><c>true</c> if this instance is in inventory the specified item; otherwise, <c>false</c>.</returns>
		/// <param name="item">Item.</param>
		public bool IsInInventory(Item item)
		{
			//search through all items in inventory for id
			for (int i=0; i<items.Count; i++) 
			{
				//this item has been found.
				if(items[i].id == item.id) return true;
			}
			//this item has not been found.
			return false;
		}
	}
}
