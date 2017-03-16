using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Inventory
{
	public class ItemSlot : MonoBehaviour, IDropHandler, ISelectHandler, IDeselectHandler
	{
		private int id;
		private Inventory inv;
		private InventoryInfo info;

		public int ID
		{
			get { return id; }
			set { id = value; }
		}

		public Inventory Inventory
		{
			get { return inv; }
			set { inv = value; }
		}

		void Start()
		{
			info = GameObject.Find("InfoPanel").GetComponent<InventoryInfo>();
		}

		public void OnSelect(BaseEventData eventData)
		{
			//if this slot is empty, and the info panel is being shown, hide it
			if(Inventory == null) print("null");
			if(Inventory.items[id].id == -1)
			{
				//can't show because no item
				info.CanShow = false;
				info.SetInfo(new Item());
				info.Toggle();
				return;
			}

			info.CanShow = true;
			info.SetInfo(eventData.selectedObject.transform.GetChild(0).GetComponent<ItemData>().Item);
		}

		public void OnDeselect(BaseEventData eventData)
		{
			info.SetInfo(new Item());
		}

		public void OnDrop(PointerEventData eventData)
		{
			ItemData droppedItem = eventData.pointerDrag.GetComponent<ItemData>();

			//if this slot is empty...
			if(Inventory.items[id].id == -1)
			{
				//on item move, replace former slot item with empty one
				Inventory.items[droppedItem.slot] = new Item();
				//set being dragged item slot to this slot
				Inventory.items[id] = droppedItem.Item;
				droppedItem.slot = id;
			}
			//swap items if slot already occupied
			else if(droppedItem.slot != id)
			{
				//get my object and set it to the dragged item's last slot
				Transform item =  this.transform.GetChild(0);
				item.GetComponent<ItemData>().slot = droppedItem.slot;
				item.transform.SetParent(Inventory.slots[droppedItem.slot].transform);
				item.localPosition = Vector2.zero;

				//set the dragged item's slot to this
				droppedItem.slot = id;
				droppedItem.transform.SetParent(this.transform);
				droppedItem.transform.localPosition = Vector2.zero;

				//swap location of items in inventory list
				Inventory.items[droppedItem.slot] = item.GetComponent<ItemData>().Item;
				Inventory.items[id] = droppedItem.Item;
			}
		}
	}
}