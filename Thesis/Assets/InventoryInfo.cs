using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory
{
	public class InventoryInfo : MonoBehaviour 
	{
		bool canShow = false;
		bool shown = false;
		bool moving = false;
		private Item item;
		public CanvasGroup cg;
		public RectTransform myRect;
		public Text title;
		public Text description;
		public Text power;
		public Text defense;
		public Text vitality;
		public float moveSpeed = 8;
		float t;

		public bool CanShow
		{
			get { return canShow; }
			set { canShow = value; }
		}

		public bool Shown
		{
			get { return shown; }
		}

		void Awake()
		{
			PlayerInput.onLeftTrigger += Toggle;
			PauseMenu.pause += CannotShow;
			PauseMenu.unPause += HideInfo;
		}

		void CannotShow() { canShow = false; }

		public void Toggle()
		{
			//not shown and can't be shown so don't
			if(!PauseMenu.paused)  return;
			if(!canShow && !shown) return;
			if(moving) StopCoroutine("Move");
			StartCoroutine("Move");
		}

		void HideInfo()
		{
			if(!shown) return;
			if(moving) StopCoroutine("Move");
			StartCoroutine("Move");
		}

		Color RarityColor(string rarity)
		{
			ItemRarity r;
			if(rarity != null)
			{
				r = (ItemRarity)System.Enum.Parse(typeof(ItemRarity), rarity);
				switch(r)
				{
				case ItemRarity.common:
				return Color.white;
				case ItemRarity.uncommon:
				return Color.yellow;
				case ItemRarity.rare:
				return new Color(1,0,1);
				case ItemRarity.legendary:
				return Color.red;
				default:
				return Color.white;
				}
			}
			else
			{
				r = ItemRarity.common;
				return Color.white;
			}
		}

		public void SetInfo(Item i)
		{
			item = i;
			title.color = RarityColor(i.rarity);
			title.text = i.title;
			description.text = i.description;
			//stats
			if(i.id != -1)
			{
				power.text = i.stats[0].ToString();
				defense.text = i.stats[1].ToString();
				vitality.text = i.stats[2].ToString();
			}
		}

		// moves info panel behind and to the side of the main
		// does a min/max x comparison with menu window and moves til met
		IEnumerator Move()
		{
			if(!shown) cg.TurnOn();
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
			if(!shown)cg.TurnOff();
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
	}
}
