using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
	/// <summary>
	/// This class is used to hold all parms associated with our database.
	/// Currently this only includes the item list.
	/// </summary>
	[System.Serializable]
	public class Database
	{
		public List<Item> items;
	}

	/// <summary>
	/// The Item Database is used to load from JSON all items into an item list.
	/// It is also used to retrieve types of items and specific items by id or string.
	/// </summary>
	public class ItemDatabase : MonoBehaviour
	{
		//simple singleton
		public static ItemDatabase Instance = null;
		//our public database
		public Database database;

		void Awake()
		{
			//create singleton once on game; DontDestroyOnLoad(); called from LevelManager
			if(Instance == null) Instance = this;
			//clear the item database when we return to the menu scene
			LevelManager.onGoToMenu += Clear;
		}

		/// <summary>
		/// Clears the database of all items.
		/// </summary>
		public void Clear()
		{
			database.items.Clear();
			Debug.Log("Cleared database.");
		}

		/// <summary>
		/// Fetches an item by a passed id int.
		/// </summary>
		/// <returns>The item.</returns>
		/// <param name="id">Identifier.</param>
		public Item FetchItem(int id)
		{
			//look through all items in database for id
			for(int i=0;i<database.items.Count;i++)
			{
				if(database.items[i].id == id) return database.items[i];
			}
			return null;
		}

		/// <summary>
		/// Fetches and item by passed slug string.
		/// </summary>
		/// <returns>The item.</returns>
		/// <param name="slug">Slug.</param>
		public Item FetchItem(string slug)
		{
			//look through all items in database for item with passed slug
			for(int i=0;i<database.items.Count;i++)
			{
				//return first item of slug string match
				if(database.items[i].slug == slug) return database.items[i];
			}
			//return null; no items were found of slug
			return null;
		}

		/// <summary>
		/// Fetches a list of items by their slug string.
		/// </summary>
		/// <returns>The item list by slug.</returns>
		/// <param name="slug">Slug.</param>
		public List<Item> FetchItemListBySlug(string slug)
		{
			//create a new list of type items
			List<Item> found = new List<Item>();
			//for all items in database
			for(int i=0;i<database.items.Count;i++)
			{
				//add item of string match to list
				if(database.items[i].slug == slug) found.Add(database.items[i]);
			}
			//return null if list is empty
			if(found.Count == 0) { Debug.Log("none found"); return null; }
			//return found if at least one item was found
			return found;
		}

		/// <summary>
		/// Creates a new item database and returns a JSON string.
		/// </summary>
		/// <returns>The new item database.</returns>
		public string CreateNewItemDatabase()
		{
			//fill database with inventory.json
			JsonUtility.FromJsonOverwrite(StaticMethods.GetStreamingAsset("Inventory.json"), database);
			//create the inventory after database created
			Inventory.Instance.ConstructInventory();
			Debug.Log("created new database");
			return JsonUtility.ToJson(database);
		}

		/// <summary>
		/// Returns the current JSON database. Creates one if non-existant.
		/// </summary>
		/// <returns>The my database.</returns>
		public string GetMyDatabase()
		{
			//create new item database if none already exists
			if(database.items.Count == 0) CreateNewItemDatabase();
			Debug.Log("saving database");
			return JsonUtility.ToJson(this.database);
		}

		/// <summary>
		/// Loads our item database from our save game JSON string.
		/// </summary>
		public void LoadItemDatabase()
		{
			//load JSON item database from our save game
			JsonUtility.FromJsonOverwrite(SaveSystem.thisSaveGame.inventory, database);
			//create the inventory after database created
			Inventory.Instance.ConstructInventory();
			Debug.Log("loaded database");
		}
	}
}
