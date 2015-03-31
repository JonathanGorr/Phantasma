using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

public class HitPoints : MonoBehaviour {

	//the next position to instantiate a point prefab(at the next harmed entity)
	private Vector2 nextPosition;

	//change the UI canvas transform
	public Canvas prefab;

	//access the UI text component
	public Text text;

	//assign colors
	public Color red = new Color (1, 0, 0);
	public Color green = new Color (0, 1, 0);

	//how much damage to print
	private float damage;

	//offset points from entity center
	public Vector3 offset = new Vector3(0f, 1.5f, 0f);
	
	private Vector2 random;

	public void TakeDamage(int value)
	{
		//pick a new location
		random = new Vector2(Random.Range(-0.6f,0.6f), 0);

		nextPosition = transform.position + offset;
		text.color = Color.red;
		text.text = "-" + value.ToString();
		Instantiate(prefab, nextPosition + random, transform.rotation);
	}

	public void Heal(int value)
	{
		//pick a new location
		random = new Vector2(Random.Range(-0.6f,0.6f), 0);

		nextPosition = transform.position + offset;
		text.color = Color.green;
		text.text = "+" + value.ToString();
		Instantiate(prefab, nextPosition + random, transform.rotation);
	}
}