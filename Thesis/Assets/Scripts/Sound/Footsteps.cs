using UnityEngine;
using System.Collections;
using CharacterController;

public class Footsteps : MonoBehaviour {

	public AudioClip[] feetForest;
	public AudioClip[] feetWood;
	public AudioClip[] feetStone;

	public CharacterController2D _controller;

	private bool step = true;
	public float audioStepLengthWalk = 0.45f;
	public float audioStepLengthRun = 0.25f;
	
	void OnTriggerEnter2D (Collider2D col) 
	{
		if (	_controller.isGrounded 
		    && _controller.velocity.magnitude < 7 
		    && _controller.velocity.magnitude > 0.1 
		    && col.gameObject.tag == "Stone"
		    && step == true) {

			StartCoroutine(WalkOnStone(audioStepLengthWalk));
		}
		else if (	_controller.isGrounded 
		    && _controller.velocity.magnitude < 7 
		    && _controller.velocity.magnitude > 0.1 
		    && col.gameObject.tag == "Forest"  
		    && step == true) {
			
			StartCoroutine(WalkOnLeaves(audioStepLengthWalk));
		}
		else if (	_controller.isGrounded 
		         && _controller.velocity.magnitude < 7 
		         && _controller.velocity.magnitude > 0.1 
		         && col.gameObject.tag == "Wood"  
		         && step == true) {
			
			StartCoroutine(WalkOnWood(audioStepLengthWalk));
		}
	}

	/////////////////////////////////// CONCRETE ////////////////////////////////////////
	IEnumerator WalkOnStone(float length) {
		step = false;
		GetComponent<AudioSource>().clip = feetStone[Random.Range (0, feetStone.Length)];
		GetComponent<AudioSource>().volume = .1f;
		GetComponent<AudioSource>().Play();
		yield return new WaitForSeconds (length);
		step = true;
	}
	
	IEnumerator RunOnStone(float length) {
		step = false;
		GetComponent<AudioSource>().clip = feetStone[Random.Range (0, feetStone.Length)];
		GetComponent<AudioSource>().volume = .3f;
		GetComponent<AudioSource>().Play();
		yield return new WaitForSeconds (length);
		step = true;
	}
	/////////////////////////////////// Forest ////////////////////////////////////////
	IEnumerator WalkOnLeaves(float length) {
		step = false;
		GetComponent<AudioSource>().clip = feetForest[Random.Range (0, feetForest.Length)];
		GetComponent<AudioSource>().volume = .5f;
		GetComponent<AudioSource>().Play();
		yield return new WaitForSeconds (length);
		step = true;
	}
	/////////////////////////////////// HardWood Floor ////////////////////////////////////////
	IEnumerator WalkOnWood(float length) {
		step = false;
		GetComponent<AudioSource>().clip = feetWood[Random.Range (0, feetWood.Length)];
		GetComponent<AudioSource>().volume = .5f;
		GetComponent<AudioSource>().Play();
		yield return new WaitForSeconds (length);
		step = true;
	}
}
