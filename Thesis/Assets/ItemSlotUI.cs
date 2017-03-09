using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour {

	public int stackLimit = 16;
	public Text countText;
	[SerializeField] private int count;
	public Button button;
	[SerializeField] private Item myItem;
	public Image icon;

	void Awake()
	{
		UpdateCount();
	}

	public void SetItem(Item item)
	{
		icon.sprite = Resources.Load<Sprite>("InventorySprites/" + item.slug);
		count ++;
		UpdateCount();
	}

	public void Consume()
	{
		count --;

		UpdateCount();
	}

	void UpdateCount()
	{
		countText.text = count.ToString();
	}
}
