using UnityEngine;
using System.Collections;

public class KillParticles : MonoBehaviour {

	bool played = false;
	public ParticleSystem ps;
	void Awake()
	{
		if(!ps) ps = GetComponent<ParticleSystem>();
	}
	// Update is called once per frame
	void Update () 
	{
		if(ps.isPlaying && !played) played = false;
		if(!played) return;
		if(!ps.IsAlive()) Destroy(this.gameObject);
	}
}
