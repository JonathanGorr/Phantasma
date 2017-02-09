using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour {

	public SFX _sfx;
	public int stackLimit = 16;
	public Text countText;
	[SerializeField] private int count;
	public Button button;
	[SerializeField] private Item myItem;
	public Image icon;

	void Awake()
	{
		count = Random.Range(0, 5);
		if(count == 0) Destroy(icon.gameObject);
		UpdateCount();
	}

	public void SetItem(Item item)
	{
		icon.sprite = Resources.Load<Sprite>("InventorySprites/" + item.SpriteID);
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
