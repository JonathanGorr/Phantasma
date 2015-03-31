using UnityEngine;
using System.Collections;

public class Particles : MonoBehaviour {

	public string LayerName = "Particles";
	public int SortingNumber = 0;

	public void Start()
	{
		particleSystem.renderer.sortingLayerName = LayerName;
		particleSystem.renderer.sortingOrder = SortingNumber;
	}
}
