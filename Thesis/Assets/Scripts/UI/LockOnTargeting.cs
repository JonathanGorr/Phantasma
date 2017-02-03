using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LockOnTargeting : MonoBehaviour {

	//create a list of all enemies found
	public List<Transform> targets;
	public Transform target;
	private Transform myTransform;
	public bool selecting = false;
	private Vector2 selectedEnemyPosition;
	private int index;
	private bool instantiate = true;
	private Transform cam;
	private Transform player;
	private Vector2 center;
	public GameObject lockOnIconPrefab;
	public bool on;

	void Awake()
	{
		//initialize
		targets = new List<Transform>();
		//have nothing targeted at script start
				target = null;
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
				target = TargetEnemy(1);
				print("index moves up");
			}
			else if(Input.GetAxis("360_RightStickHorizontal") < -0.9)
			{
				target = TargetEnemy(-1);
				print("index moves down");
			}
		}
	}
	
	void FixedUpdate()
	{
		if(target != null)
		{
			if(instantiate == true)
			{
				//make a retical, set it to invisible by default
				Instantiate(lockOnIconPrefab, target.transform.position, target.transform.rotation);

				//set to false when one is made
				instantiate = false;
			}

				center = ((target.position - player.transform.position)/2.0f) + target.position;
			cam.transform.LookAt(center);
		}
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

	private Transform TargetEnemy(int direction)
	{
		//if there are no selected targets, sort them by distance
		//and set the selectedTarget as the closest[0]
		if(target == null)
		{
			SortTargetsByDistance();
			return targets[0];
		}
		else
		{
			//gives us the index of the current target selected
			index = targets.IndexOf(target);

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
			return targets[index];
		}
	}
}
