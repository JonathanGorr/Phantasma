using UnityEngine;
using System.Collections;

public class GlowSprite : MonoBehaviour {
	
	public float duration = 1f;
	public SpriteRenderer sprite;
	
	void Update() {
		float a = Mathf.PingPong (Time.time / duration, 1.0f);
		sprite.color = new Color(1f, 1f, 1f, a);
	}
}