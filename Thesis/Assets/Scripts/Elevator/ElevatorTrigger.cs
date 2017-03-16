using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Elevator trigger:
/// Calls the elevator the floor associated with this trigger
/// </summary>

public enum Floor { floorGround, floor1 }

public class ElevatorTrigger : MonoBehaviour {

	public Floor myFloor;
	public Elevator elevator;

	void OnTriggerEnter2D(Collider2D col)
	{
		if(myFloor == elevator.currentFloor) return;

		if(col.CompareTag("Player"))
		{
			elevator.Call(myFloor);
		}
	}
}
