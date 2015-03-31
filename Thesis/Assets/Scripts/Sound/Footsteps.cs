﻿using UnityEngine;
using System.Collections;

public class Footsteps : MonoBehaviour {

	public AudioClip[] feetForest;
	public AudioClip[] feetWood;
	public AudioClip[] feetStone;

	private CharacterController2D _controller;

	private bool step = true;
	public float audioStepLengthWalk = 0.45f;
	public float audioStepLengthRun = 0.25f;

	void Awake()
	{
		_controller = GameObject.Find("_Player").GetComponent<CharacterController2D>();
	}
	
	void OnTriggerEnter2D (Collider2D col) {
		
		if (	_controller.isGrounded 
		    && _controller.velocity.magnitude < 7 
		    && _controller.velocity.magnitude > 0.1 
		    && col.gameObject.tag == "Ground"
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
		audio.clip = feetStone[Random.Range (0, feetStone.Length)];
		audio.volume = .1f;
		audio.Play();
		yield return new WaitForSeconds (length);
		step = true;
	}
	
	IEnumerator RunOnStone(float length) {
		step = false;
		audio.clip = feetStone[Random.Range (0, feetStone.Length)];
		audio.volume = .3f;
		audio.Play();
		yield return new WaitForSeconds (length);
		step = true;
	}
	/////////////////////////////////// Forest ////////////////////////////////////////
	IEnumerator WalkOnLeaves(float length) {
		step = false;
		audio.clip = feetForest[Random.Range (0, feetForest.Length)];
		audio.volume = .5f;
		audio.Play();
		yield return new WaitForSeconds (length);
		step = true;
	}
	/////////////////////////////////// HardWood Floor ////////////////////////////////////////
	IEnumerator WalkOnWood(float length) {
		step = false;
		audio.clip = feetWood[Random.Range (0, feetWood.Length)];
		audio.volume = .5f;
		audio.Play();
		yield return new WaitForSeconds (length);
		step = true;
	}
}
