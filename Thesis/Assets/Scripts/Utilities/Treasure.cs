using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory;

public class Treasure : MonoBehaviour {

	public bool debug = false;
	Inventory.Inventory _inventory;
	public bool randomCount = true;
	public GameObject treasure;
	public float upForce = 50;
	public int count;
	public int min;
	public int max;

	public List<Item> currency = new List<Item>();

	void Start()
	{
		_inventory = (Inventory.Inventory)GameObject.FindObjectOfType(typeof(Inventory.Inventory));
		//get all (coins) in our database
		currency = _inventory.database.FetchItemListBySlug("coin");
	}

	public void Dispense()
	{
		if(randomCount) count = Random.Range(min, max);
		if(count == 0) return;

		int[] change = MakeChange(count);
		//do for each currency type
		for(int i=0; i<currency.Count;i++)
		{
			//early out if no change is desired from this coin value
			if(change[i] == 0) continue;
			//get this currency coin
			GameObject coin = (GameObject) Resources.Load("Prefabs/Items/" + currency[i].slug + currency[i].id);
			//spawn change[i] number of those coins
			for(int j=0;j<change[i];j++)
			{
				coin = Instantiate(coin, this.transform.position, Quaternion.identity);
				coin.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.value/4, Random.value) * upForce);
			}
		}
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.N) && count > 0) MakeChange(count);
	}

	//returns a change array of the number of valued coins that should be spawned
	int[] MakeChange(int amt)
	{
		int golds = (int)amt/currency[2].value;
		if(golds > 0) { amt = amt % currency[2].value; }

		int silvers = (int)amt/currency[1].value;
		if(silvers > 0) { amt = amt % currency[1].value; }

		//remainder in pennies
		int coppers = (int)amt;

		if(debug) print("Total: " + count + "\n" + "Golds: " + golds + " Silvers: " + silvers + " Coppers: " + coppers);
		return new int[] { coppers, silvers, golds};
	}
}