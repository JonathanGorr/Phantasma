using UnityEngine;
using System.Collections;

public class KillParticles : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		if(!GetComponent<ParticleSystem>().IsAlive()) Destroy(gameObject);
	}
}
