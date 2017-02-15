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
		private Inventory inv;
		private Transform originalParent;
		Vector2 offset;

		void Start()
		{
			inv = GameObject.Find("Inventory").GetComponent<Inventory>();
		}

		public void UpdateCount()
		{
			circle.color = amount <= 1 ? Color.clear: Color.white;
			text.text = amount <= 1 ? "" : amount.ToString();
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			if(Item == null) return;
			offset = eventData.position - new Vector2(this.transform.position.x, this.transform.position.y);
			originalParent = this.transform.parent;
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
			this.transform.SetParent(inv.slots[slot].transform);
			this.transform.position = inv.slots[slot].transform.position;
			cg.blocksRaycasts = true;
		}
	}
}
