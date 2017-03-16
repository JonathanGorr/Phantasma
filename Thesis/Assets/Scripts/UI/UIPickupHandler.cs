using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// User interface pickup handler:
/// Used to indicate if there's an item on the ground that the player can pick up
/// Shows the pickup dialog when a pickup trigger is entered
/// Interacts with the playerInfo script
/// </summary>

public class UIPickupHandler : MonoBehaviour {

	public static UIPickupHandler Instance = null;

	public Animator pickupDialog;
	public RectTransform pickupListWindow;
	public Animator pickupListAnim;
	public CanvasGroup pickupListCG;
	public GameObject itemPanelPrefab;

	void Awake()
	{
		if(Instance == null) Instance = this; 
	}

	//hide/show pickup dialog
	public void HidePickup() {
		pickupDialog.SetBool("Shown", false);
	}
	public void RevealPickup() {
		pickupDialog.SetBool("Shown", true);
	}

	public void Pickup(List<int> itemList)
	{
		StartCoroutine(CreatePickupList(itemList));
	}

	//takes the list of items on the drop and uses it to create a UI list
	IEnumerator CreatePickupList(List<int> itemList)
	{
		pickupListAnim.SetBool("Shown", true);
		for(int i=0;i<itemList.Count;i++)
		{
			//create ui thing and add to list
			GameObject itemPanel = Instantiate(itemPanelPrefab, pickupListWindow) as GameObject;
			RectTransform rect = itemPanel.GetComponent<RectTransform>();
			rect.localScale = Vector3.one;
			itemPanel.GetComponent<PickupItemUI>().SetItem(itemList[i]);
		}

		yield return new WaitForSeconds(2);
		pickupListAnim.SetBool("Shown", false);

		//wait until invisible to clear UI list
		while(pickupListCG.alpha > 0) yield return null;
		foreach(Transform child in pickupListWindow)
		{
			Destroy(child.gameObject);
		}
	}
}
