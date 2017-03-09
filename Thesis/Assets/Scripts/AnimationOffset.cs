using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationOffset : MonoBehaviour {

	public Animator anim;

	void Awake()
	{
		anim.speed = Random.Range(0.8f, 1f);
	}
}
