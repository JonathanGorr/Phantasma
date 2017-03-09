using UnityEngine;
using System.Collections;

/// <summary>
/// Checkpoint.
/// Triggers a conversation, progresses the story( and saves it ).
/// Methods here can be used to deactivate the trigger as well as activate/deactivate colliders like doors
/// </summary>

public class Checkpoint : MonoBehaviour {

	public bool
		motherMet,
		fatherMet,
		returnedHome,
		itemsGet,
		swordTut,
		bowTut,
		puzzle,
		boss;

	public QuestCollider[] questColliders;
	public Collider2D myTrigger;

	[HideInInspector] public ConversationComponent dialog;
	[HideInInspector] public bool completed = false;

	//when the player enters the checkpoint, his location(checkpoint) is saved to the prefs
	void OnTriggerEnter2D(Collider2D col)
	{
		if(!col.CompareTag("Player")) return;
		if(completed) return;
		completed = true;

		PlayerPreferences.Instance.SaveStats();
		PlayerPreferences.Instance.SavePlayerPosition();
		GameObject.Find("CheckpointReached").GetComponent<Animator>().SetTrigger("CheckpointReached");

		//set bools in prefs accordingly
		if(motherMet)
		{
			PlayerPreferences.Instance.MotherMet();
		}
		if(fatherMet)
		{
			PlayerPreferences.Instance.FatherMet();
		}
		if(itemsGet)
		{
			PlayerPreferences.Instance.ItemsGet();
		}
		if(swordTut)
		{
			PlayerPreferences.Instance.SwordTutorialFinished();
		}
		if(returnedHome)
		{
			PlayerPreferences.Instance.WheresMomFinished();
		}
		if(bowTut)
		{
			PlayerPreferences.Instance.BowTutorialFinished();
		}
		if(puzzle)
		{
			PlayerPreferences.Instance.PuzzleFinished();
		}
		if(boss)
		{
			PlayerPreferences.Instance.BossFinished();
		}

		StartCoroutine(WaitTilDoneTalking());
	}

	IEnumerator WaitTilDoneTalking()
	{
		while(ConversationManager.Instance.talking) yield return null;
		DisableColliders();
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawCube(transform.position,new Vector2(1,1));
	}

	//destroy this object
	public void Destroy()
	{
		Destroy(this.gameObject);
	}

	//activates my collider
	public virtual void ActivateTrigger()
	{
		myTrigger.enabled = true;
	}

	//deactivates my collider
	public virtual void DeactivateTrigger()
	{
		myTrigger.enabled = false;
	}

	//'opens all related doors' or otherwise impeads progress with colliders
	public void EnableColliders()
	{
		for(int i=0;i<questColliders.Length;i++)
		{
			questColliders[i].Enable();
		}
	}

	//'opens all related doors' or otherwise removes colliders impeading progress
	public void DisableColliders()
	{
		for(int i=0;i<questColliders.Length;i++)
		{
			questColliders[i].Disable();
		}
	}
}
