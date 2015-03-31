using UnityEngine;
using System.Collections;

public class KillParticles : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		if(!particleSystem.IsAlive()) Destroy(gameObject);
	}
}
