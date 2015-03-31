using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LockOnTargeting : MonoBehaviour {

	//create a list of all enemies found
	public List<Transform> targets;
	public Transform selectedTarget;
	private Transform myTransform;
	public bool selecting = false;
	public GameObject prefab;
	private Vector2 selectedEnemyPosition;
	private int index;
	private FollowEntity _follow;
	private bool instantiate = true;
	private Transform cam;
	private Transform player;
	private Vector2 center;
	public bool on;

	void Awake()
	{
		//initialize
		targets = new List<Transform>();
		//have nothing targeted at script start
		selectedTarget = null;
		//cache transform if used often
		myTransform = transform;
		//start adding immediately
		AddAllEnemies();
		//find cam
		cam = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Transform>();
		player = GameObject.Find("_Player").GetComponent<Transform>();
	}

	void Update()
	{
		if(on)
		{
			//on each button press, toggle retical on/off
			if(Input.GetButtonDown("360_RightThumbStickButton"))
			{
				selecting = !selecting;
				TargetEnemy(1);
			}
		}
		//on each scroll, move position of retical
		if(selecting == true)
		{
			if(Input.GetAxis("360_RightStickHorizontal") > 0.9f)
			{
				TargetEnemy(1);
				print("index moves up");
			}
			else if(Input.GetAxis("360_RightStickHorizontal") < -0.9)
			{
				TargetEnemy(-1);
				print("index moves down");
			}
		}
	}
	
	void FixedUpdate()
	{
		if(selectedTarget != null)
		{
			if(instantiate == true)
			{
				//make a retical, set it to invisible by default
				Instantiate(prefab, selectedTarget.transform.position, selectedTarget.transform.rotation);
				_follow = prefab.GetComponent<FollowEntity> ();

				//set to false when one is made
				instantiate = false;
			}

			center = ((selectedTarget.position - player.transform.position)/2.0f) + selectedTarget.position;
			cam.transform.LookAt(center);
		}
		//else
			//print("there is no selected target");
	}

	//add all enemies(tag) to list
	public void AddAllEnemies()
	{
		GameObject[] go = GameObject.FindGameObjectsWithTag ("Enemy");

		foreach(GameObject enemy in go)
		{
			AddTarget(enemy.transform);
		}
	}

	//used for adding only enemy transforms to list
	public void AddTarget(Transform enemy)
	{
		targets.Add(enemy);
	}

	private void SortTargetsByDistance()
	{
		//gets distance between two transforms, returns float
		targets.Sort(delegate(Transform t1, Transform t2) {
			return Vector3.Distance(t1.position, myTransform.position).CompareTo(Vector3.Distance(t2.position, myTransform.position));
     		});
	}

	private void TargetEnemy(int direction)
	{
		//if there are no selected targets, sort them by distance
		//and set the selectedTarget as the closest[0]
		if(selectedTarget == null)
		{
			SortTargetsByDistance();
			selectedTarget = targets[0];
		}
		else
		{
			//gives us the index of the current target selected
			index = targets.IndexOf(selectedTarget);

			if(direction == 1)
			{
				index ++;
			}
			else if(direction == -1)
			{
				index --;
			}
			/*
			//iterates through available targets
			if(index < targets.Count - 1)
			{
				index++;
			}
			else
			{
				index = 0;
			}
			*/
			//DeselectTarget();
			selectedTarget = targets[index];
		}
		//show the lock on sprite
		SelectTarget();
	}

	private void SelectTarget()
	{
		_follow.target = selectedTarget;
	}
}
