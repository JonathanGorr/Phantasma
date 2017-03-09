using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;

namespace Inventory
{
	[System.Serializable]
	public class Database
	{
		public List<Item> items;
	}

	public class ItemDatabase : MonoBehaviour
	{
		public static ItemDatabase Instance = null;

		public Database database;

		void Awake()
		{
			if(Instance == null) Instance = this;
			LevelManager.onGoToMenu += Clear;
		}

		public void Clear()
		{
			print("cleared database");
			database.items.Clear();
		}

		Item Search<T>(List<T> list, int id) where T : Item
		{
			for(int i=0;i<list.Count;i++)
			{
				if(list[i].id == id) return list[i];
			}
			return null;
		}

		Item Search<T>(List<T> list, string slug) where T : Item
		{
			for(int i=0;i<list.Count;i++)
			{
				if(list[i].slug == slug) return list[i];
			}
			return null;
		}

		//by id
		public Item FetchItem(int id)
		{
			Item item = null;
			while(item == null)
			{
				item = Search(database.items, id);
				break;
			}
			return item;
		}

		//by slug string
		public Item FetchItem(string slug)
		{
			Item item = null;
			while(item == null)
			{
				item = Search(database.items, slug);
				break;
			}
			return item;
		}

		public List<Item> FetchItemListBySlug(string slug)
		{
			List<Item> found = new List<Item>();
			for(int i=0;i<database.items.Count;i++)
			{
				if(database.items[i].slug == slug) found.Add(database.items[i]);
			}
			if(found.Count == 0) { /*print("none found");*/ return null; }
			return found;
		}

		//copies streaming assets
		public string CreateNewItemDatabase()
		{
			//fill database with inventory.json
			JsonUtility.FromJsonOverwrite(StaticMethods.GetStreamingAsset("Inventory.json"), database);
			//create the inventory after database created
			Inventory.Instance.ConstructInventory();
			#if UNITY_EDITOR
			print("created new database");
			#endif
			return JsonUtility.ToJson(database);
		}

		//returns the current database as a json string for saving
		public string GetMyDatabase()
		{
			//create new item database if none already exists
			if(database.items.Count == 0) CreateNewItemDatabase();
			#if UNITY_EDITOR
			print("saving database");
			#endif
			return JsonUtility.ToJson(this.database);
		}

		//creates database from save game
		public void LoadItemDatabase()
		{
			JsonUtility.FromJsonOverwrite(SaveSystem.thisSaveGame.inventory, database);

			//create the inventory after database created
			Inventory.Instance.ConstructInventory();
			#if UNITY_EDITOR
			print("loaded database");
			#endif
		}
	}
}
