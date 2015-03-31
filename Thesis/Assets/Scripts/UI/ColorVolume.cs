using UnityEngine;
using System.Collections;

public class ColorVolume : MonoBehaviour {

	private DayNightSystem _cycle;

	void Awake()
	{
		_cycle = GameObject.Find ("_LevelManager").GetComponent<DayNightSystem>();
	}

	void OnTriggerEnter2D(Collider2D target)
	{
		if (target.gameObject.tag == "Castle")
			_cycle.Castle();
		else if(target.gameObject.tag == "Cemetary")
			_cycle.Cemetary();
	}

	void OnTriggerExit2D(Collider2D target)
	{
		if(target.gameObject.tag == "Castle")
			_cycle.Outside();
		else if(target.gameObject.tag == "Cemetary")
			_cycle.Outside();
	}
}
