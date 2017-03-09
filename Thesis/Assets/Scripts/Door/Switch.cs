using UnityEngine;
using System.Collections;

public class Switch : MonoBehaviour {

	//set up array
	public DoorTrigger[] doorTriggers;

	//toggle whether you want the button to stay down(sticky) on first depression
	//or be spring returned like a pressure plate
	public bool sticky;

	private bool down;
	private Animator animator;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator> ();
	}

	void OnTriggerEnter2D (Collider2D target)
	{
		if(target.gameObject.tag == "Player")
		{
			animator.SetInteger ("AnimState", 1);
			//switch is down
			down = true;

			foreach(DoorTrigger trigger in doorTriggers)
			{
				if(trigger != null)
					trigger.Toggle(true);
			}
		}
	}

	void OnTriggerExit2D (Collider2D target)
	{
		//when the player walks off the switch, and its set to sticky
		//the switch will not raise/reset itself
		if (sticky && down)
			return;

		animator.SetInteger ("AnimState", 2);
		//switch is no longer down
		down = false;

		foreach(DoorTrigger trigger in doorTriggers)
		{
			if(trigger != null)
				trigger.Toggle(false);
		}
	}

	//draws a gizmo that draws a line between an associated switch and door
	void OnDrawGizmos()
	{
		//changes color of line depending on if its sticky or not
		Gizmos.color = sticky ? Color.red : Color.green;
		//goes through all triggers in array
		foreach(DoorTrigger trigger in doorTriggers)
		{
			if(trigger != null) 
				if(trigger.door)
					Gizmos.DrawLine(transform.position, trigger.door.transform.position);
		}

	}
}
