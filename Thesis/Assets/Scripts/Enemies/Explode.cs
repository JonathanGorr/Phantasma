using UnityEngine;
using System.Collections;

public class Explode : MonoBehaviour {

	public AnimationMethods animMethods;
	public GameObject itemPrefab;
	public int spawnCount = 3;
	private float waitTime = .3f;

	void Awake()
	{
		animMethods.onCollapse += OnExplode;
	}

	public void OnExplode()
	{
		animMethods.onCollapse -= OnExplode;
		StartCoroutine(DoExplode());
	}

	IEnumerator DoExplode()
	{
		for(int i=0; i<spawnCount; i++)
		{
			SFX.Instance.PlayFX("item_spawn", transform.position);
			transform.TransformPoint(0,-100,0);
			GameObject clone = Instantiate(itemPrefab, transform.position, Quaternion.identity) as GameObject;
			//add some size variation
			float random = Random.Range(0.8f, 1f);
			clone.transform.localScale = new Vector2(random, random);
			//random x
			clone.GetComponent<Rigidbody2D>().AddForce(Vector3.right * Random.Range(-3,3));
			//random y
			clone.GetComponent<Rigidbody2D>().AddForce(Vector3.up * Random.Range(30,40));
			yield return new WaitForSeconds(waitTime);
		}
	}
}
