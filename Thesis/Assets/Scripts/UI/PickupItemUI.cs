using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Sets the blank ui slot to an assigned item
/// </summary>

public class PickupItemUI : MonoBehaviour {

	public Image icon;
	public Text title;
	public Text description;

	public void SetItem(int itemID)
	{
		Item item = Inventory.ItemDatabase.Instance.FetchItem(itemID);
		icon.sprite = Resources.Load<Sprite>("Sprites/Items/" + item.slug);
		title.text = item.title;
		description.text = item.description;
	}
}
