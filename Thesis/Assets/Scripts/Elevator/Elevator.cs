using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Elevator:
/// This is a basic elevator script for moving the player vertically
/// When the player is on the elevator, press A to go up.

/// If the player is on a level which the elevator is currently not,
/// call the elevator to the player's level
/// </summary>

public class Elevator : MonoBehaviour {

	public Floor currentFloor = Floor.floorGround;

	Transform myTransform;
	public float upY = 7.372f;
	public float downY = 2.84f;
	public AudioSource asrc;

	bool playerIsOn = false;
	bool moving = false;

	public float moveTime = 3; //seconds

	void Awake()
	{
		myTransform = transform;
		PlayerInput.onA += Toggle;
	}

	//is the elevator in the up location?
	public bool IsUp {
		get { return myTransform.position.y == upY; }
	}
	//is the elevator in the down location?
	public bool IsDown {
		get { return myTransform.position.y == downY; }
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if(col.CompareTag("Player"))
		{
			playerIsOn = true;
		}
	}

	void OnTriggerExit2D(Collider2D col)
	{
		if(col.CompareTag("Player"))
		{
			playerIsOn = false;
		}
	}

	//move elevator to the passed floor
	public void Call(Floor floor)
	{
		if(moving) return;

		if((int)currentFloor > (int)floor)
			StartCoroutine(MoveDown());
		else
			StartCoroutine(MoveUp());
	}

	//explicitly goes down
	public void CallDown()
	{
		if(moving) return;
		if(IsDown) return;

		SFX.Instance.PlayFX("elevatorButton_Pressed", myTransform.position);
		StartCoroutine(MoveDown());
	}

	//explicitly goes up
	public void CallUp()
	{
		if(moving) return;
		if(IsUp) return;

		SFX.Instance.PlayFX("elevatorButton_Pressed", myTransform.position);
		StartCoroutine(MoveUp());
	}

	//goes up or down based on current state
	public void Toggle()
	{
		if(moving) return;
		if(!playerIsOn) return;

		SFX.Instance.PlayFX("elevatorButton_Pressed", myTransform.position);
		if(IsUp) StartCoroutine(MoveDown());
		else StartCoroutine(MoveUp());
	}

	IEnumerator MoveUp()
	{
		moving = true;

		yield return new WaitForSeconds(.5f);
		asrc.Play();

		float t = 0;
		while(t <= 1.0f)
		{
			t += Time.deltaTime/moveTime;
			myTransform.position = Vector2.Lerp(
				new Vector2(myTransform.position.x, downY),
				new Vector2(myTransform.position.x, upY),
				Mathf.SmoothStep(0.0f, 1.0f, t));

			yield return null;
		}

		currentFloor += 1;
		moving = false;

		asrc.Stop();
	}

	IEnumerator MoveDown()
	{
		moving = true;	

		yield return new WaitForSeconds(.5f);
		asrc.Play();

		float t = 0;
		while(t <= 1.0f)
		{
			t += Time.deltaTime/moveTime;
			myTransform.position = Vector2.Lerp(
				new Vector2(myTransform.position.x, upY),
				new Vector2(myTransform.position.x, downY),
				Mathf.SmoothStep(0.0f, 1.0f, t));

			yield return null;
		}

		currentFloor -= 1;
		moving = false;

		asrc.Stop();
	}
}
