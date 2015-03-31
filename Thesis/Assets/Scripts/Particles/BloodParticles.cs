using UnityEngine;
using System.Collections;

public class BloodParticles : MonoBehaviour {

	public ParticleSystem bloods;
	
	void Update () 
	{    
		if (Input.GetKeyDown(KeyCode.Z))
		{
			FireBloodParticles();
		}
	}
	
	void FireBloodParticles()
	{
		Vector3 position = transform.position + new Vector3(0,0,-0.1f);
		ParticleSystem localBloodsObj = GameObject.Instantiate(bloods, position, bloods.transform.rotation) as ParticleSystem;
		localBloodsObj.Play();
	}
}
