using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// An enemy instantiator.
/// </summary>

public class Spawner : MonoBehaviour {

	public GameObject[] enemyPool;
	public float delay = 2.0f;
	//public bool active = true;
	private Vector2 direction = new Vector2(1, 1);
	private List<GameObject> targets;
	private Gizmo parentGizmo;

	// Use this for initialization
	void Start () {
		parentGizmo = gameObject.GetComponent<Gizmo>();
		targets = parentGizmo.targets;
		StartCoroutine(EnemyGenerator());
	}

	IEnumerator EnemyGenerator()
	{
		if (enabled)
		{
			var newTransform = transform;
			
			yield return new WaitForSeconds(delay);
			
			if (targets.Count > 0)
			{
				var spawnTarget = targets[Random.Range(0, targets.Count)];
				newTransform = spawnTarget.transform;
				direction = spawnTarget.transform.localScale;
			}
			
			GameObject clone = Instantiate(enemyPool[Random.Range(0,
			                                                      enemyPool.Length)], newTransform.position, Quaternion.identity) 
				as GameObject;
			
			clone.transform.localScale = direction;
			
			StartCoroutine(EnemyGenerator());
		}
	}
}
