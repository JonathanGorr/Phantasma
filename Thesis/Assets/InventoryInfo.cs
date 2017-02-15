using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory
{
	public class InventoryInfo : MonoBehaviour {

		public PlayerInput _input;
		private LevelManager _manager;
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
			_manager = GameObject.Find("_LevelManager").GetComponent<LevelManager>();
			_input.onLeftTrigger += Toggle;
			LevelManager.pause += CannotShow;
			LevelManager.unPause += HideInfo;
		}

		void CannotShow() { canShow = false; }

		public void Toggle()
		{
			//not shown and can't be shown so don't
			if(!_manager.paused)  return;
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

		Color RarityColor(Item.ItemRarity rarity)
		{
			switch(rarity)
			{
				case Item.ItemRarity.common:
				return Color.black;
				break;
				case Item.ItemRarity.uncommon:
				return Color.yellow;
				break;
				case Item.ItemRarity.rare:
				return new Color(1,0,1);
				break;
				case Item.ItemRarity.legendary:
				return Color.red;
				break;
				default:
				return Color.black;
				break;
			}
		}

		public void SetInfo(Item i)
		{
			item = i;
			title.text = item.Title;
			title.color = RarityColor(i.Rarity);
			description.text = item.Description;
			power.text = item.Power.ToString();
			defense.text = item.Defense.ToString();
			vitality.text = item.Vitality.ToString();
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
	}
}
