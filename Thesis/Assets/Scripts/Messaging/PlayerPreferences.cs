using UnityEngine;
using System.Collections;

public class PlayerPreferences : MonoBehaviour {

	public bool inMenu;
	private Transform _player;
	private Evolution _evo;
	private Health _health;
	private WeaponSwitcher _switcher;
	private FollowEntity _skull;
	public GameObject[] dialog;
	public GameObject[] checkpoints;
	[HideInInspector]
	public Transform _spawn;

	public bool 
		motherMet,
		fatherMet,
		itemsGet,
		swordLesson,
		returnedHome,
		bowLesson,
		puzzle;

	void Awake()
	{
		if(!inMenu)
		{
			//assign all the loaded variables saved in prefs
			_player = GameObject.Find("_Player").transform;
			//new game position
			_spawn = GameObject.Find("Spawn").transform;
			//blood
			_evo = GameObject.Find("_LevelManager").GetComponent<Evolution>();
			_skull = GameObject.Find("Father").GetComponent<FollowEntity>();
			_switcher = GetComponent<WeaponSwitcher>();
			_health = _player.GetComponent<Health>();

			//update story after all references gathered
			UpdateStory();

			//blood
			_evo.blood = PlayerPrefs.GetInt("Blood");

			//if a game has been created, load health
			if(PlayerPrefs.GetInt("GameCreated") == 1)
			{
				//location
				_player.position = new Vector3(PlayerPrefs.GetFloat("playerX"), PlayerPrefs.GetFloat("playerY"), 0);
				_health.health = PlayerPrefs.GetInt("Health");
			}
			//else if a new game has just been created, set health to max
			else
			{
				_health.health = _health.maxHealth;
				//location
				_player.position = _spawn.position;
				GameCreated();
			}
		}
	}

	public void UpdateStory()
	{
		if(!inMenu)
		{
			//wheres mom dialog and checkpoint
			if(dialog[3] != null)
				dialog[3].SetActive(false);
			if(checkpoints[2] != null)
				checkpoints[2].SetActive(false);
			//sword lesson and dialog
			if(checkpoints[1] != null)
				checkpoints[1].SetActive(false);
			if(dialog[2] != null)
				dialog[2].SetActive(false);
			
			//story sequence checkers
			if(PlayerPrefs.GetInt("MotherMet") == 1)
			{
				motherMet = true;
			}
			else
			{
				motherMet = false;
			}
			
			//player should have no weapons
			//mother dialog should be destroyed, first checkpoint should be disabled
			if(PlayerPrefs.GetInt("FatherMet") == 1)
			{
				fatherMet = true;
				//destroy mom and dad dialog
				Destroy(dialog[0]);
				Destroy(dialog[1]);
				//destroy dad checkpoint
				Destroy (checkpoints[0]);
				
				//set sword tut and checkpoint to true
				if(dialog[2] != null)
					dialog[2].SetActive(true);
				if(checkpoints[1] != null)
					checkpoints[1].SetActive(true);
				
				//set weapons to on
				if(_switcher != null)
					_switcher.weaponGet = true;
				else
					print("switcher is missing");
			}
			else
			{
				fatherMet = false;
			}
			
			//sword tut
			if(PlayerPrefs.GetInt("SwordTutorial") == 1)
			{
				swordLesson = true;

				if(dialog[3] != null)
					dialog[3].SetActive(true);
				if(checkpoints[2] != null)
					checkpoints[2].SetActive(true);
				
				//destroy sword tut
				Destroy(dialog[2]);
				Destroy(checkpoints[1]);
			}
			else
			{
				swordLesson = false;
				print("sword tut never done");
			}
			
			//mother should be destroyed, father dialog should be destroyed
			//player should have weapons from now on
			if(PlayerPrefs.GetInt("ReturnedHome") == 1)
			{
				returnedHome = true;
				Destroy(dialog[3]);
				Destroy(checkpoints[2]);
			}
			else
			{
				returnedHome = false;
			}
			
			//bow tut
			if(PlayerPrefs.GetInt("BowTutorial") == 1)
			{
				bowLesson = true;
				Destroy(dialog[4]);
				Destroy(checkpoints[3]);
			}
			else
			{
				bowLesson = false;
				print("bow tut never done");
			}

			//puzzle dialog
			if(PlayerPrefs.GetInt("Puzzle") == 1)
			{
				puzzle = true;
				Destroy(dialog[5]);
				Destroy(checkpoints[6]);
			}
			else
			{
				puzzle = false;
			}

			//items and dialog
			if(PlayerPrefs.GetInt("ItemsGet") == 1)
			{
				itemsGet = true;
				_switcher.weaponGet = true;
				_skull.spiritGet = true;
			}
			else
			{
				itemsGet = false;
			}
		}
	}

	void Update()
	{
		if(!inMenu)
		{
			if(_player != null)
			{
				if(Input.GetKeyDown(KeyCode.Return))
				{
					EraseAll();
				}

				if(Input.GetKeyDown(KeyCode.P))
				{
					SaveStats(_player.position.x, _player.position.y, _evo.blood, _health.health);
				}
			}
		}
	}

	public void SaveStats(float x, float y, int blood, int health)
	{
		PlayerPrefs.SetFloat ("playerX", x);
		PlayerPrefs.SetFloat ("playerY", y);
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
		_switcher.weaponGet = true;
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
