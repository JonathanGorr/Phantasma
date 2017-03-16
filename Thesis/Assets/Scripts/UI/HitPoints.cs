using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

public class HitPoints : MonoBehaviour {

	//change the UI canvas transform
	public Canvas prefab;
	//access the UI text component
	public Text text;
	//assign colors
	public Color damageColor = new Color (1, 0, 0);
	public Color healColor = new Color (0, 1, 0);

	//offset points from entity center
	public Vector3 offset = new Vector3(0f, 1.5f, 0f);
	
	Vector3 RandomSpawn() //returns a random spawn location
	{
	 	return new Vector3(Random.Range(-0.2f,0.2f), 0, 0);
	}

	public void TakeDamage(int value)
	{
		text.color = damageColor;
		text.text = "-" + value.ToString();
		Instantiate(prefab, transform.position + offset + RandomSpawn(), transform.rotation);
	}

	public void Heal(int value)
	{
		text.color = healColor;
		text.text = "+" + value.ToString();
		Instantiate(prefab, transform.position + offset + RandomSpawn(), transform.rotation);
	}
}