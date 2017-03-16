using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Animation offset:
/// A utility that "offsets" or changes the animation speed 
/// of this instance of a spawned object's animator so as to 
/// differentiate or vary animations among identical objects.
/// </summary>

public class AnimationOffset : MonoBehaviour {

	public Animator anim;

	void Awake()
	{
		anim.speed = Random.Range(0.8f, 1f);
	}
}
