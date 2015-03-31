using UnityEngine;
using System.Collections;

public class FadeOutSprite : MonoBehaviour {

	private SpriteRenderer spriteRenderer;
	private Color start;
	private Color end;
	public float delay = -5f;

	void Start ()
	{
		spriteRenderer = GetComponent<SpriteRenderer> ();
		start = spriteRenderer.color;
		//rgba
		end = new Color (start.r, start.g, start.b, 0.0f);
	}

	void Update()
	{
		//tell time
		delay += Time.deltaTime;

		//fade sprite out over time
		renderer.material.color = Color.Lerp (start, end, delay/2);

		//destroy object if not visible
		if(renderer.material.color.a <= 0.0)
		{
			Destroy(gameObject);
		}
	}
}
