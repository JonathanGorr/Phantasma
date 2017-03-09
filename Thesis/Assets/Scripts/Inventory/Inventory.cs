using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

namespace Inventory
{
	public class Inventory : MonoBehaviour 
	{
		public static Inventory Instance = null;

		public InventoryInfo _info;
		public PauseMenu _menu;
		public RectTransform inventoryPanel;
		public ItemDatabase database;
		public RectTransform itemPrefab;
		public RectTransform slotPrefab;
		bool inInventory = false;
		public Text coinText;
		int coins;

		public int Coins
		{
			get { return coins; }
			set { coins = value; }
		}

		public void AddCoins(int count)
		{
			coins += count;
			UpdateCoinUI();
		}

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

		public void ConstructInventory()
		{
			//create slots
			if(slots.Count == 0)
			{
				//add slots
				for(int i=0; i<slotAmount;i++)
				{
					items.Add(new Item()); // creates a new, empty item for this slot
					RectTransform rt = Instantiate(slotPrefab, inventoryPanel);
					rt.localScale = Vector3.one;
					rt.name = "Slot " + i;
					slots.Add(rt);
					ItemSlot slot = slots[i].GetComponent<ItemSlot>();
					slot.ID = i;
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
				//add 1 item by id for each
				//print("Added: " + ItemDatabase.Instance.database.items[i].title + " " + ItemDatabase.Instance.database.items[i].count);
				//spawn count of each item
				for(int j=0; j<ItemDatabase.Instance.database.items[i].count;j++)
				{
					//add 1 item by id for each
					AddItem(i, false);
				}
			}
		}

		#if UNITY_EDITOR
		void Update()
		{
			if(Input.GetKeyDown(KeyCode.K))
				AddItem(6, true);
			if(Input.GetKeyDown(KeyCode.L))
				AddItem(5, true);
		}
		#endif

		public bool AddItem(int id, bool addToDatabase)
		{
			Item itemToAdd = ItemDatabase.Instance.FetchItem(id);

			if(itemToAdd.slots)
			{
				if(itemToAdd.stackable && IsInInventory(itemToAdd)) //stack if present already
				{
					for(int i=0; i<items.Count; i++) 
					{
						if(items[i].id == itemToAdd.id)
						{
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
				else //the item is not stackable or isn't in inventory
				{
					//add items to slots
					for(int i=0; i<items.Count; i++)
					{
						if(items[i].id == -1) //if we found an empty inventory slot
						{
							items[i] = itemToAdd;
							RectTransform itemObj = Instantiate(itemPrefab, slots[i].transform);

							//zero out position and scale
							RectTransform rt = itemObj.GetComponent<RectTransform>();
							rt.localPosition = Vector2.zero;
							rt.sizeDelta = Vector2.zero;

							itemObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Items/" + itemToAdd.slug);
							ItemData data = itemObj.GetComponent<ItemData>();
							data.slot = i;
							data.amount ++;
							data.Item = itemToAdd;
							data.UpdateCount();
							if(addToDatabase) itemToAdd.count ++;
							return true;
						}
					}

					SFX.Instance.PlayUI("error");
					print("inventory full");
					return false;
				}
			}
			if(addToDatabase) itemToAdd.count ++;
			return true;
		}

		//is item in inventory?
		public bool IsInInventory(Item item)
		{
			for (int i=0; i<items.Count; i++) 
			{
				if(items[i].id == item.id) return true;
			}
			return false;
		}
	}
}
