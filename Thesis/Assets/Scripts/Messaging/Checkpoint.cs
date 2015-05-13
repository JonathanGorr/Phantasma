using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour {

	private PlayerPreferences _prefs;
	private GameObject _checkpointMessage;
	private Animator _anim;
	private GameObject _manager;
	private LevelManager _managerComp;
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
		puzzle;

	public float convoDistance = 2;

	// Use this for initialization
	void Awake () {
		_target = GameObject.Find ("_Player").transform;
		_manager = GameObject.Find ("_LevelManager");
		_prefs = _manager.GetComponent<PlayerPreferences>();
		_managerComp = GameObject.Find ("_LevelManager").GetComponent<LevelManager>();
		_playerGO = GameObject.Find("_Player");
		_evo = _manager.GetComponent<Evolution>();
		_playerHealth = _playerGO.GetComponent<Health>();

		//if not in menu, find the checkpoint and its animated text
		if(!_managerComp.inMenu)
		{
			_checkpointMessage = GameObject.Find("Checkpoint");
			_anim = GameObject.Find("CheckpointReached").GetComponent<Animator>();
		}
		else
			print("in menu");
	}

	//when the player enters the checkpoint, his location(checkpoint) is saved to the prefs
	void Update()
	{
		float distance = Vector3.Distance (transform.position, _target.position);

		if(distance < convoDistance)
		{
			if(_prefs)
			{
				_prefs.SaveStats(transform.position.x, transform.position.y, _evo.blood, _playerHealth.health);
				StartCoroutine(ShowMessage());

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
			}
			else
				print ("prefs could not be found");
		}
	}

	IEnumerator ShowMessage()
	{
		if(_anim != null)
		{
			_anim.SetTrigger("CheckpointReached");
			print ("checkpoint reached");
		}
		else
			print("theres no animator");

		yield return new WaitForSeconds(2f);

		if(_checkpointMessage != null)
			_checkpointMessage.SetActive (false);

		Destroy (gameObject);
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawCube(transform.position,new Vector2(1,1));
	}
}
