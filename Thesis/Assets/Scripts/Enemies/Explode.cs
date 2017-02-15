using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Explode : MonoBehaviour {

	public GameObject itemPrefab;
	private int spawnCount;

	public void OnExplode()
	{
		for (int i = 0; i < spawnCount; i++)
		{
			transform.TransformPoint(0,-100,0);
			GameObject clone = Instantiate(itemPrefab, transform.position, Quaternion.identity) as GameObject;
			clone.GetComponent<Rigidbody2D>().AddForce(Vector3.right * Random.Range(-50,50));
			clone.GetComponent<Rigidbody2D>().AddForce(Vector3.up * Random.Range(100,400));
		}

		Destroy(gameObject);
	}
}
