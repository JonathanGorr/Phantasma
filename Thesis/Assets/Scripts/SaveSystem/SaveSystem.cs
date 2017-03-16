using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using UnityEngine.SceneManagement;

public static class SaveSystem {

	public static int currentSaveFile = 0;
	public static MySaveGame thisSaveGame = null;

	public delegate void OnSaveGame();
	public static event OnSaveGame save;

	public delegate void OnLoadGame();
	public static event OnLoadGame load;

	//creates a new saveGame class and passes it to the serialization method
	public static void Save(int i)
	{
		Debug.Log("saved");
		if(save != null) save();
		MySaveGame game = DoesSaveGameExist(i) ? LoadGame(i) : new MySaveGame();

		if(game == null) Debug.Log("is null");
		game.inventory = Inventory.ItemDatabase.Instance.GetMyDatabase();
		game.quests = QuestSystem.QuestManager.Instance.GetMyDatabase();

		//save these
		game.coins = Inventory.Inventory.Instance.Coins;
		game.blood = Evolution.Instance.Blood;

		//store a copy of the current save game
		thisSaveGame = game;
		SaveThisGame(game, i);
	}

	//takes a saveGame and saves it to file with currentSaveFile extension
	private static bool SaveThisGame(MySaveGame game, int i)
	{
		BinaryFormatter formatter = new BinaryFormatter();

		using (FileStream stream = new FileStream(GetSavePath(i), FileMode.Create))
		{
			formatter.Serialize(stream, game);
		}

		return true;
	}

	//deserializes current save game
	public static MySaveGame LoadGame(int i)
	{
		if (!DoesSaveGameExist(i))
		{
			#if UNITY_EDITOR
			Debug.Log("save game doesn't exist");
			#endif
			return null;
		}

		BinaryFormatter formatter = new BinaryFormatter();

		using (FileStream stream = new FileStream(GetSavePath(i), FileMode.Open))
		{
			try
			{
				return formatter.Deserialize(stream) as MySaveGame;
			}
			catch (Exception)
			{
				#if UNITY_EDITOR
				Debug.Log("This save game cannot be loaded.");
				#endif
				return null;
			}
		}
	}

	//takes the serialized stream from current save game and applies it
	public static void Load(int i)
	{
		if(load != null) load(); //load event
		thisSaveGame = LoadGame(i);

		Inventory.ItemDatabase.Instance.LoadItemDatabase();
	}

	public static bool DeleteSaveGame(int i)
	{
		try
		{
			#if UNITY_EDITOR
			Debug.Log("Deleted: " + GetSavePath(i));
			#endif

			File.Delete(GetSavePath(i));
		}
		catch (Exception)
		{
			#if UNITY_EDITOR
			Debug.Log("Theres no save game yet : " + GetSavePath(i));
			#endif

			return false;
		}

		return true;
	}

	public static bool DoesSaveGameExist(int i)
	{
		return File.Exists(GetSavePath(i));
	}

	private static string GetSavePath(int i)
	{
		return Path.Combine(Application.persistentDataPath, "SaveGame" + i.ToString() + ".phantasma");
	}

	//this is a template class for different save files, if needed
	[Serializable]		
	public abstract class SaveGame
	{
	}

	//this is, by default, a new game
	[Serializable]
	public class MySaveGame : SaveGame
	{
		//player
		public string playerName = "";
		public string className = "";
		public int maxHealth = 10;
		public Color[] playerPalette;
		public float playerX;
		public float playerY;

		//jsons
		public string quests;
		public string inventory;

		//stats
		public int blood = 0;
		public int coins = 0;

		//options-------------

		//audio
		public float fxVolume = 0.5f;
		public float musicVolume = 0.5f;
		public bool fxMuted = false;
		public bool musicMuted = false;
	}
}