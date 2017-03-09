using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Inventory
{
	public class ItemData : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

		[HideInInspector] public int slot;
		public CanvasGroup cg;
		public Image circle;
		public Text text;
		public Item Item { get; set; }
		public int amount;
		Vector2 offset;

		public void UpdateCount()
		{
			circle.color = amount <= 1 ? Color.clear: Color.white;
			text.text = amount <= 1 ? "" : amount.ToString();
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			if(Item == null) return;
			offset = eventData.position - new Vector2(this.transform.position.x, this.transform.position.y);
			this.transform.SetParent(this.transform.parent.parent.parent);
			this.transform.position = eventData.position - offset;
			cg.blocksRaycasts = false;
		}

		public void OnDrag(PointerEventData eventData)
		{
			if(Item == null) return;
			this.transform.position = eventData.position - offset;
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			//grab the transform of the slot assigned to my item
			this.transform.SetParent(Inventory.Instance.slots[slot].transform);
			this.transform.position = Inventory.Instance.slots[slot].transform.position;
			cg.blocksRaycasts = true;
		}
	}
}
