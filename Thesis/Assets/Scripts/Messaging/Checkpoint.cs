using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour {

	private PlayerPreferences _prefs;
	private Animator _anim;
	private LevelManager _manager;
	private Evolution _evo;
	private GameObject _playerGO;
	private Health _playerHealth;
	private Transform _target;
	public bool
		motherMet,
		fatherMet,
		returnedHome,
		itemsGet,
		swordTut,
		bowTut,
		puzzle,
		visited;

	public float distance = 2;

	// Use this for initialization
	void Start () 
	{
		_target = GameObject.Find ("_Player").transform;
		_manager = GameObject.Find ("_LevelManager").GetComponent<LevelManager>();
		_prefs = _manager.GetComponent<PlayerPreferences>();
		_playerGO = GameObject.Find("_Player");
		_evo = _manager.GetComponent<Evolution>();
		_playerHealth = _playerGO.GetComponent<Health>();

		//if not in menu, find the checkpoint and its animated text
		_anim = GameObject.Find("CheckpointReached").GetComponent<Animator>();
	}

	//when the player enters the checkpoint, his location(checkpoint) is saved to the prefs
	void Update()
	{
		if(!_target) return;
		float d = Vector3.Distance (transform.position, _target.position);

		if(!visited)
		{
			if(d < distance)
			{
				if(_prefs)
				{
					_prefs.SaveStats(transform.position, _evo.blood, _playerHealth.health);
					_anim.SetTrigger("CheckpointReached");

					//set bools in prefs accordingly
					if(motherMet)
					{
						_prefs.MotherMet();
					}
					else if(fatherMet)
					{
						_prefs.FatherMet();
					}
					else if(itemsGet)
					{
						_prefs.ItemsGet();
					}
					else if(swordTut)
					{
						_prefs.SwordTutorialFinished();
					}
					else if(returnedHome)
					{
						_prefs.ReturnedHome();
					}
					else if(bowTut)
					{
						_prefs.BowTutorialFinished();
					}
					else if(puzzle)
					{
						_prefs.Puzzle();
					}

					visited = true;
				}
			}
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawCube(transform.position,new Vector2(1,1));
	}
}
