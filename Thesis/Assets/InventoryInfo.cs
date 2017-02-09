using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory
{
	public class InventoryInfo : MonoBehaviour {

		private bool shown = false;
		bool moving = false;
		public CanvasGroup cg;
		public RectTransform myRect;
		public Text title;
		public Text description;
		public Text power;
		public Text defense;
		public Text vitality;
		public Text rarity;
		public float moveSpeed = 8;
		float t;

		public bool Shown
		{
			get { return shown; }
		}

		public void Toggle()
		{
			if(moving) StopCoroutine("Move");
			StartCoroutine("Move");
		}

		// moves info panel behind and to the side of the main
		// does a min/max x comparison with menu window and moves til met
		IEnumerator Move()
		{
			if(!shown) UI.TurnOn(cg);
			shown = !shown;
			moving = true;
			while((shown && t < 1) || (!shown && t > 0))
			{
				t += (shown == true ? Time.deltaTime : -Time.deltaTime) * moveSpeed;
				myRect.anchoredPosition = 
					Vector2.Lerp(
						new Vector2( (shown == true ? myRect.sizeDelta.x: -myRect.sizeDelta.x)/2, myRect.anchoredPosition.y),
						new Vector2( (shown == true ? -myRect.sizeDelta.x: myRect.sizeDelta.x)/2, myRect.anchoredPosition.y), shown ? t : 1-t);
				yield return null;
			}
			t = Mathf.Clamp(t, 0, 1);
			moving = false;
			if(!shown)UI.TurnOff(cg);
		}

		public string Title
		{
			set { title.text = value; }
		}

		public string Description
		{
			set { description.text = value; }
		}

		public string Power
		{
			set { power.text = value; }
		}

		public string Defense
		{
			set { defense.text = value; }
		}

		public string Vitality
		{
			set { vitality.text = value; }
		}

		public string Rarity
		{
			set { rarity.text = value; }
		}
	}
}
