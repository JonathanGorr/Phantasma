using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Pickup:
/// This is an unknown item revealed on pickup.
/// A small dialog or icon of some sort will indicate the item.

/// When the player is standing over this, allow pickup and disable jumping- as activated on A
/// </summary>

public class Pickup : MonoBehaviour {

	public int[] items;
	public int maxItems = 3;

	void Start()
	{
		//spawn a random number of random items
		items = new int[Random.Range(1,maxItems)];
		for(int i=0; i<items.Length; i++)
		{
			items[i] = Random.Range(0, Inventory.ItemDatabase.Instance.database.items.Count);
		}
	}
}
