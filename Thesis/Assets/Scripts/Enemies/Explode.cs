using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Explode : MonoBehaviour {

	public Blood blood;
	public int enemyBlood;
	private int totalSpawned;
	private int playerBlood;

	void Start()
	{
		playerBlood = GameObject.Find("_LevelManager").GetComponent<Evolution>().blood;
	}

	public void OnExplode()
	{
		if(transform.tag == "Player")
			totalSpawned = playerBlood;
		else
		{
			totalSpawned = enemyBlood;
			Destroy(gameObject);
		}

		var t = transform;

		for (int i = 0; i < totalSpawned; i++)
		{
			t.TransformPoint(0,-100,0);
			Blood clone = Instantiate(blood, t.position, Quaternion.identity) as Blood;
			clone.GetComponent<Rigidbody2D>().AddForce(Vector3.right * Random.Range(-50,50));
			clone.GetComponent<Rigidbody2D>().AddForce(Vector3.up * Random.Range(100,400));
		}
	}
}
