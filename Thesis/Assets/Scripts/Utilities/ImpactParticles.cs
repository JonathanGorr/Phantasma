using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactParticles : MonoBehaviour {

	public ParticleSystem particles;

	void OnTriggerEnter2D(Collider2D col)
	{
		if(col.CompareTag("Wood") || col.CompareTag("Forest") || col.CompareTag("Stone"))
		{
			SFX.Instance.PlayFX("impact_" + tag, transform.position);
		}
		particles.Emit(10);
	}
}
