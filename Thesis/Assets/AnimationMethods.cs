using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationMethods : MonoBehaviour {

	private SFX _sfx;

	void Start()
	{
		_sfx = GameObject.Find("_LevelManager").GetComponent<SFX>();
	}

	 public void SwingSoundLight()
	 {
	 	_sfx.PlayFX("swing_Light", transform.position);
	 } 

	 public void SwingSoundHeavy()
	 {
	 	_sfx.PlayFX("swing_Heavy", transform.position);
	 }
}
