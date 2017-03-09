using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// Level specific story sequence based on playerPrefs.
/// Outdated; would prefer a modular quest system that might replace later...
/// </summary>

public class PlayerPreferences : MonoBehaviour {

	public static PlayerPreferences Instance = null;

	public bool debug = false;

	public Checkpoint MeetMom;
	public Checkpoint MeetDad;
	public Checkpoint SwordLesson;
	public Checkpoint WheresMom;
	public Checkpoint BowLesson;
	public Checkpoint Puzzle;
	public Checkpoint Boss;

	[Header("Story Progress")]
	public bool motherMet;
	public bool	fatherMet;
	public bool	itemsGet;
	public bool	swordLesson;
	public bool	returnedHome;
	public bool	bowLesson;
	public bool	puzzle;
	public bool	boss;

	void Awake()
	{
		#if !UNITY_EDITOR
		debug = false;
		#endif

		Instance = this;
		StartCoroutine(Initialize());
	}

	IEnumerator Initialize()
	{
		while(!Player.Instance) yield return null;

		MeetMom.ActivateTrigger();
		//wheres mom dialog and checkpoint
		WheresMom.DeactivateTrigger();
		//initially disable sword lesson and dialog
		SwordLesson.DeactivateTrigger();

		//LOAD GAME
		if(PlayerPrefs.GetInt("GameCreated") == 1)
		{
			if(debug) print("Prefs: game loaded.");

			//SEQUENCE:
			//The Player.Instance is dropped in
			//The Player.Instance can go left or right
			//If the Player.Instance goes left, empy graveyard
			//If the Player.Instance goes right, talks to mom, reveals father
			//If the Player.Instance talks to the father, the mother object is destroyed

			//call all to update
			if(PlayerPrefs.GetInt("MotherMet") == 1) MotherMet();
			if(PlayerPrefs.GetInt("FatherMet") == 1) FatherMet();
			if(PlayerPrefs.GetInt("ItemsGet") == 1) ItemsGet();
			if(PlayerPrefs.GetInt("SwordTutorial") == 1) SwordTutorialFinished();
			if(PlayerPrefs.GetInt("WheresMom") == 1) WheresMomFinished();
			if(PlayerPrefs.GetInt("BowTutorial") == 1) BowTutorialFinished();
			if(PlayerPrefs.GetInt("Puzzle") == 1) PuzzleFinished();
			if(PlayerPrefs.GetInt("Boss") == 1) BossFinished();

			Player.Instance.transform.position = new Vector3(PlayerPrefs.GetFloat("PlayerX"), PlayerPrefs.GetFloat("PlayerY"), 0);
			Player.Instance._health.health = PlayerPrefs.GetInt("Health");

			Inventory.Inventory.Instance.Coins = PlayerPrefs.GetInt("Coins");
			Inventory.Inventory.Instance.UpdateCoinUI();

			Evolution.Instance.Blood = PlayerPrefs.GetInt("Blood");
			Evolution.Instance.UpdateUI();
		}

		//NEW GAME
		else
		{
			if(debug) print("Prefs: new game just created");
			Player.Instance._health.health = Player.Instance._health.maxHealth;
			Player.Instance.SetPosition(GameObject.FindWithTag("Respawn").transform.position);

			Inventory.Inventory.Instance.Coins = 0;
			Inventory.Inventory.Instance.UpdateCoinUI();

			Evolution.Instance.Blood = 0;
			Evolution.Instance.UpdateUI();
			GameCreated();
		}
	}

	//SEQUENCE:
	//The Player.Instance is dropped in
	//The Player.Instance can go left or right
	//If the Player.Instance goes left, empy graveyard
	//If the Player.Instance goes right, talks to mom, reveals father
	//If the Player.Instance talks to the father, the mother object is destroyed

	#if UNITY_EDITOR
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.O))
		{
			print("Data Saved.");
			SaveStats();
		}
		else if(Input.GetKeyDown(KeyCode.P)) 
		{
			print("Data Erased.");
			 EraseAll(); 
	 	}
	}
	#endif

	void OnApplicationQuit()
	{
		if(SceneManager.GetActiveScene().name != "Start") return;
		if(!Player.Instance._health.Dead) SaveStats();
	}

	public void SavePlayerPosition()
	{
		PlayerPrefs.SetFloat ("PlayerX", Player.Instance.transform.position.x);
		PlayerPrefs.SetFloat ("PlayerY", Player.Instance.transform.position.y);
	}

	public void SaveStats()
	{
		PlayerPrefs.SetInt ("Blood", Evolution.Instance.Blood);
		PlayerPrefs.SetInt ("Coins", Inventory.Inventory.Instance.Coins);
		PlayerPrefs.SetInt ("Health", Player.Instance._health.health);
		PlayerPrefs.Save();
	}

	public void GameCreated()
	{
		PlayerPrefs.SetInt("GameCreated", 1);
		PlayerPrefs.Save();
	}

	public void MotherMet()
	{
		MeetDad.ActivateTrigger();
		motherMet = true;

		PlayerPrefs.SetInt("MotherMet", 1);
		PlayerPrefs.Save();
	}

	public void FatherMet()
	{
		fatherMet = true;
		//destroy mom and dad dialog
		MeetDad.DisableColliders();
		//set sword tut and checkpoint to true
		SwordLesson.ActivateTrigger();
		//destroy mom
		if(MeetMom) MeetMom.Destroy();

		PlayerPrefs.SetInt("FatherMet", 1);
		PlayerPrefs.Save();
	}

	public void ItemsGet()
	{
		itemsGet = true;
		GameObject.Find("Father").GetComponent<FollowEntity>().SetTarget(Player.Instance);

		PlayerPrefs.SetInt("ItemsGet", 1);
		PlayerPrefs.Save();
	}

	public void SwordTutorialFinished()
	{
		MeetDad.DeactivateTrigger();
		swordLesson = true;
		WheresMom.ActivateTrigger();

		PlayerPrefs.SetInt("SwordTutorial", 1);
		PlayerPrefs.Save();
	}

	public void WheresMomFinished()
	{
		SwordLesson.DeactivateTrigger();
		BowLesson.ActivateTrigger();
		returnedHome = true;

		PlayerPrefs.SetInt("ReturnedHome", 1);
		PlayerPrefs.Save();
	}

	public void BowTutorialFinished()
	{
		BowLesson.Destroy();
		Puzzle.ActivateTrigger();
		PlayerPrefs.SetInt("BowTutorial", 1);
		PlayerPrefs.Save();
	}

	public void PuzzleFinished()
	{
		BowLesson.DeactivateTrigger();
		Boss.ActivateTrigger();
		PlayerPrefs.SetInt("Puzzle", 1);
		PlayerPrefs.Save();
	}

	public void BossFinished()
	{
		BowLesson.DeactivateTrigger();
		PlayerPrefs.SetInt("Boss", 1);
		PlayerPrefs.Save();
	}

	public void EraseAll()
	{
		PlayerPrefs.DeleteAll();
	}
}
