using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySound : MonoBehaviour {

	public AudioSource asrc;

	void Update()
	{
		if(!asrc.isPlaying) Destroy(this.gameObject);
	}
}
