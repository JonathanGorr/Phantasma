using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerPreferences : MonoBehaviour {

	public class Checkpoint
	{
		public bool completed = false;
		public GameObject dialog;
		public GameObject checkpoint;
	}

	private Player _player;
	private Evolution _evo;
	private Health _health;
	private LevelManager _manager;
	private FollowEntity _skull;
	public GameObject[] dialog;
	public GameObject[] checkpoints;
	public Transform _spawn;
	private GameObject _motherTalkBlock;

	public bool
		motherMet,
		fatherMet,
		itemsGet,
		swordLesson,
		returnedHome,
		bowLesson,
		puzzle;

	void Start()
	{
		_manager = GetComponent<LevelManager>();

		if(_manager.inMenu) return;
		if(_manager.inInitialize) return;

		//assign all the loaded variables saved in prefs
		_player = GameObject.Find("_Player").GetComponent<Player>();
		_spawn = GameObject.FindWithTag("Respawn").transform;
		_evo = GameObject.Find("_LevelManager").GetComponent<Evolution>();
		_skull = GameObject.Find("Father").GetComponent<FollowEntity>();
		_health = _player.GetComponent<Health>();
		_motherTalkBlock = GameObject.Find("MomTalkBlock");

		//update story after all references gathered
		UpdateStory();
		  
		//blood
		_evo.blood = PlayerPrefs.GetInt("Blood");

		//if a game has been created, load health
		if(PlayerPrefs.GetInt("GameCreated") == 1)
		{
			//location
			_player.transform.position = new Vector3(PlayerPrefs.GetFloat("playerX"), PlayerPrefs.GetFloat("playerY"), 0);
			_health.health = PlayerPrefs.GetInt("Health");
		}
		//else if a new game has just been created, set health to max
		else
		{
			_health.health = _health.maxHealth;
			//location
			_player.transform.position = _spawn.position;
			GameCreated();
		}
	}

	//SEQUENCE:
	//The player is dropped in
	//The player can go left or right
	//If the player goes left, empy graveyard
	//If the player goes right, talks to mom, reveals father
	//If the player talks to the father, the mother object is destroyed
	public void UpdateStory()
	{
		if(_manager.inMenu) return;

		//wheres mom dialog and checkpoint
		dialog[3].SetActive(false);

		checkpoints[2].SetActive(false);

		//sword lesson and dialog
		checkpoints[1].SetActive(false);

		dialog[2].SetActive(false);
		
		//Mother met
		if(PlayerPrefs.GetInt("MotherMet") == 1)
		{
			motherMet = true;
			Destroy(_motherTalkBlock);
		}
		else
			motherMet = false;
		
		//player should have no weapons
		//mother dialog should be destroyed, first checkpoint should be disabled
		if(PlayerPrefs.GetInt("FatherMet") == 1)
		{
			fatherMet = true;
			//destroy mom and dad dialog
			Destroy(dialog[0]);
			
			//set sword tut and checkpoint to true
			dialog[2].SetActive(true);
			checkpoints[1].SetActive(true);
		}
		else
			fatherMet = false;
		
		//sword tut
		if(PlayerPrefs.GetInt("SwordTutorial") == 1)
		{
			swordLesson = true;

			dialog[3].SetActive(true);
			checkpoints[2].SetActive(true);
		}
		else
		{
			swordLesson = false;
		}
		
		//mother should be destroyed, father dialog should be destroyed
		//player should have weapons from now on
		if(PlayerPrefs.GetInt("ReturnedHome") == 1)
		{
			returnedHome = true;
		}
		else
		{
			returnedHome = false;
		}
		
		//bow tut
		if(PlayerPrefs.GetInt("BowTutorial") == 1)
		{
			bowLesson = true;
		}
		else
		{
			bowLesson = false;
		}

		//puzzle dialog
		if(PlayerPrefs.GetInt("Puzzle") == 1)
		{
			puzzle = true;
		}
		else
		{
			puzzle = false;
		}

		//items and dialog
		if(PlayerPrefs.GetInt("ItemsGet") == 1)
		{
			itemsGet = true;
			_skull.spiritGet = true;
		}
		else
		{
			itemsGet = false;
		}
	}

	void Update()
	{
		if(_manager.inMenu) return;
		if(_manager.inInitialize) return;

		if(_player)
		{
			if(Input.GetKeyDown(KeyCode.O))
			{
				print("Data Saved.");
				SaveStats(_player.transform.position, _evo.blood, _health.health);
			}
			else if(Input.GetKeyDown(KeyCode.P)) 
			{
				print("Data Erased.");
				 EraseAll(); 
		 	}
		}
	}

	void OnApplicationQuit()
	{
		if(_manager.inMenu || _manager.inInitialize) return;
		 	SaveStats(_player.transform.position, _evo.blood, _player._health.health);
	}

	public void SaveStats(Vector3 pos, int blood, int health)
	{
		PlayerPrefs.SetFloat ("playerX", pos.x);
		PlayerPrefs.SetFloat ("playerY", pos.y);
		PlayerPrefs.SetInt ("Blood", blood);
		PlayerPrefs.SetInt ("Health", health);
		UpdateStory ();
		PlayerPrefs.Save();
	}

	public void GameCreated()
	{
		PlayerPrefs.SetInt("GameCreated", 1);
		UpdateStory ();
		PlayerPrefs.Save();
	}

	public void MotherMet()
	{
		PlayerPrefs.SetInt("MotherMet", 1);
		UpdateStory ();
		PlayerPrefs.Save();
	}

	public void FatherMet()
	{
		PlayerPrefs.SetInt("FatherMet", 1);
		UpdateStory ();
		PlayerPrefs.Save();
	}

	public void ItemsGet()
	{
		PlayerPrefs.SetInt("ItemsGet", 1);
		UpdateStory ();
		PlayerPrefs.Save();
	}

	public void SwordTutorialFinished()
	{
		PlayerPrefs.SetInt("SwordTutorial", 1);
		UpdateStory ();
		PlayerPrefs.Save();
	}

	public void ReturnedHome()
	{
		PlayerPrefs.SetInt("ReturnedHome", 1);
		UpdateStory ();
		PlayerPrefs.Save();
	}

	public void BowTutorialFinished()
	{
		PlayerPrefs.SetInt("BowTutorial", 1);
		UpdateStory ();
		PlayerPrefs.Save();
	}

	public void Puzzle()
	{
		PlayerPrefs.SetInt("Puzzle", 1);
		UpdateStory ();
		PlayerPrefs.Save();
	}

	public void EraseAll()
	{
		PlayerPrefs.DeleteAll();
	}
}
