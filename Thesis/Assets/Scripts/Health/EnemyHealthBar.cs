using UnityEngine;
using System.Collections;

public class EnemyHealthBar : MonoBehaviour {

	//location of healthbar
	private float left;
	private float top;
	private Vector2 playerScreen;
	public float offsetX;
	public float offsetY;

	//compiled textures
	public Texture backgroundTexture;
	public Texture foregroundTexture;
	public Texture borderTexture;

	//declare dimensions of textures
	private int barWidth;
	private int barHeight;

	//Scale the size of the health bar
	public float scale;

	//import health component
	private Health _health;

	void Awake()
	{
		_health = GetComponent<Health>();

		//texture dimensions = healthbar dimensions
		barWidth = borderTexture.width;
		barHeight = borderTexture.height;
	}

	void FixedUpdate()
	{
		Mathf.Clamp (_health.health, 0, _health.maxHealth);

		//update the healthbar location every frame
		playerScreen = Camera.main.WorldToScreenPoint(transform.position);
		left = playerScreen.x + offsetX;
		top = Screen.height - playerScreen.y + offsetY;
	}
	
	void OnGUI()
	{
		//shrink the healthbar relative to damage
		var percent = ((double)_health.health / (double)_health.maxHealth);

		//draw the textures
		GUI.DrawTexture (new Rect (left, top, barWidth * scale, barHeight * scale), backgroundTexture);
		//scale the middle bar, but also shrink it by scale
		GUI.DrawTexture (new Rect (left, top, (int)System.Math.Round(barWidth * percent * scale), barHeight * scale), foregroundTexture);
		GUI.DrawTexture (new Rect (left, top, barWidth * scale, barHeight * scale), borderTexture);
	}
}