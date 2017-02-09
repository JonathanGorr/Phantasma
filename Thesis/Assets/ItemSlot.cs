using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Inventory
{
	public class ItemSlot : MonoBehaviour, IDropHandler 
	{
		[HideInInspector] public int id;
		private Inventory inv;

		void Start()
		{
			inv = GameObject.Find("Inventory").GetComponent<Inventory>();
		}

		public void OnDrop(PointerEventData eventData)
		{
			ItemData droppedItem = eventData.pointerDrag.GetComponent<ItemData>();

			//if this slot is empty...
			if(inv.items[id].ID == -1)
			{
				//on item move, replace former slot item with empty one
				inv.items[droppedItem.slot] = new Item();
				//set being dragged item slot to this slot
				inv.items[id] = droppedItem.Item;
				droppedItem.slot = id;
			}
			//swap items if slot already occupied
			else
			{
				//get my object and set it to the dragged item's last slot
				Transform item =  this.transform.GetChild(0);
				item.GetComponent<ItemData>().slot = droppedItem.slot;
				item.transform.SetParent(inv.slots[droppedItem.slot].transform);
				item.localPosition = Vector2.zero;

				//set the dragged item's slot to this
				droppedItem.slot = id;
				droppedItem.transform.SetParent(this.transform);
				droppedItem.transform.localPosition = Vector2.zero;

				//swap location of items in inventory list
				inv.items[droppedItem.slot] = item.GetComponent<ItemData>().Item;
				inv.items[id] = droppedItem.Item;
			}
		}
	}
}