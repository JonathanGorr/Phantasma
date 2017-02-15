using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory
{
	public class Inventory : MonoBehaviour {

		public LevelManager _manager;
		public InventoryInfo _info;
		public PauseMenu _menu;
		public RectTransform inventoryPanel;
		public ItemDatabase database;
		public RectTransform itemPrefab;
		public RectTransform slotPrefab;
		bool inInventory = false;
		public Text coinText;
		bool infoShown = false;
		int coins;

		public int Coins
		{
			get { return coins; }
			set { coins = value; }
		}

		public void AddCoins(int count)
		{
			coins += count;
			UpdateCoins();
		}

		public void UpdateCoins()
		{
			coinText.text = coins.ToString();
		}

		int slotAmount = 20;
		[HideInInspector] public List<RectTransform> slots = new List<RectTransform>();
		[HideInInspector] public List<Item> items = new List<Item>();

		void Start()
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

			AddItem(5);
			AddItem(6);
			AddItem(7);
			AddItem(5);
			AddItem(4);
			UpdateCoins();
		}

		void Update()
		{
			if(!_manager.paused) return;
			if(_menu.currentTab != Tabs.Inventory) return;
		}

		public void AddItem(int id)
		{
			Item itemToAdd = database.FetchItem(id);

			//stack if present already
			if(itemToAdd.Stackable && IsInInventory(itemToAdd))
			{
				for (int i = 0; i < items.Count; i++) 
				{
					if(items[i].ID == id)
					{
						ItemData data = slots[i].transform.GetChild(0).GetComponent<ItemData>();
						data.amount ++;
						data.UpdateCount();
						break; //don't continue
					}
				}
			}
			else
			{
				//add items to slots
				for(int i=0; i<items.Count;i++)
				{
					if(items[i].ID == -1)
					{
						items[i] = itemToAdd;
						RectTransform itemObj = Instantiate(itemPrefab, slots[i].transform);
						itemObj.localScale = Vector3.one;
						itemObj.GetComponent<Image>().sprite = itemToAdd.Sprite;
						itemObj.name = itemToAdd.Title;
						ItemData data = itemObj.GetComponent<ItemData>();
						data.amount = 1;
						data.slot = i;
						data.Item = itemToAdd;
						data.UpdateCount();
						break; //don't continue
					}
				}
			}
		}

		//is item in inventory?
		public bool IsInInventory(Item item)
		{
			for (int i = 0; i < items.Count; i++) 
			{
				if(items[i].ID == item.ID)
					return true;
			}
			return false;
		}
	}
}
