using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory
{
	public class Inventory : MonoBehaviour {

		public LevelManager _manager;
		public InventoryInfo _info;
		public PlayerInput _input;
		public PauseMenu _menu;
		public RectTransform inventoryPanel;
		public ItemDatabase database;
		public RectTransform itemPrefab;
		public RectTransform slotPrefab;

		public Text coins;

		bool infoShown = false;

		int slotAmount = 20;
		[HideInInspector] public List<RectTransform> slots = new List<RectTransform>();
		[HideInInspector] public List<Item> items = new List<Item>();

		void Awake()
		{
			LevelManager.unPause += HideInfo;
			_input.onLeftTrigger += ToggleInfo;
		}

		void Start()
		{
			if(slots.Count == 0)
			{
				//add slots
				for(int i=0; i<slotAmount;i++)
				{
					items.Add(new Item()); // creates a new, empty item for this slot
					RectTransform slot = Instantiate(slotPrefab, inventoryPanel);
					slot.localScale = Vector3.one;
					slot.name = "Slot " + i;
					slots.Add(slot);
					slots[i].GetComponent<ItemSlot>().id = i;
				}
			}

			AddItem(0);
			AddItem(1);
			AddItem(1);
			AddItem(1);
			AddItem(1);
			UpdateCoins(155);
		}

		public void UpdateCoins(int v)
		{
			coins.text = v.ToString();
		}

		void Update()
		{
			if(!_manager.paused) return;
			if(_menu.currentTab != Tabs.Inventory) return;
		}

		void ToggleInfo()
		{
			_info.Toggle();
		}

		void HideInfo()
		{
			if(_info.Shown) _info.Toggle();
		}

		public void AddItem(int id)
		{
			Item itemToAdd = database.FetchItemByID(id);

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
						break;//don't continue
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
