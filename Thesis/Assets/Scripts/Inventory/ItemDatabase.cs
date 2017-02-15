using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;

namespace Inventory
{
	public class ItemDatabase : MonoBehaviour {

		private List<Item> database = new List<Item>();
		private JsonData itemData;

		void Awake()
		{
			//load a json file into this container
			itemData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Items.json"));
			ConstructItemDatabase();
		}

		//by id
		public Item FetchItem(int id)
		{
			for(int i = 0;i<database.Count;i++)
			{
				if(database[i].ID == id) return database[i];
			}
			return null;
		}

		//by slug string
		public Item FetchItem(string slug)
		{
			for(int i = 0;i<database.Count;i++)
			{
				if(database[i].Slug == slug) return database[i];
			}
			return null;
		}

		public List<Item> FetchItemsBySlug(string slug)
		{
			List<Item> found = new List<Item>();
			for(int i=0;i<database.Count;i++)
			{
				if(database[i].Slug == slug) found.Add(database[i]);
			}
			if(found.Count == 0) { print("none found"); return null; }
			return found;
		}

		void ConstructItemDatabase()
		{
			//creates a new item for each entry in the json
			for(int i=0;i<itemData.Count;i++)
			{
				//get values by json identifier and contruct items with them
				database.Add(new Item(
				(int)itemData[i]["id"],
				(string)itemData[i]["title"],
				(string)itemData[i]["description"],
				(int)itemData[i]["value"],
				(bool)itemData[i]["slots"],

				(int)itemData[i]["stats"]["power"],
				(int)itemData[i]["stats"]["defense"],
				(int)itemData[i]["stats"]["vitality"],

				(bool)itemData[i]["stackable"],
				(string)itemData[i]["rarity"],
				(string)itemData[i]["slug"]));
			}
		}
	}
}
