using UnityEngine;
using System.Collections;

public class Particles : MonoBehaviour {

	public string LayerName = "Particles";
	public int SortingNumber = 0;

	public void Start()
	{
		GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingLayerName = LayerName;
		GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingOrder = SortingNumber;
	}
}
