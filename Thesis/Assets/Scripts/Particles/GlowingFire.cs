using UnityEngine;
using System.Collections;

public class GlowingFire : MonoBehaviour {

	/**
     * Coroutine to create a flash effect on all sprite renderers passed in to the function.
     *
     * @param sprites   a sprite renderer array
     * @param numTimes  how many times to flash
     * @param delay     how long in between each flash
     * @param disable   if you want to disable the renderer instead of change alpha
     */

	//flashing
	public int flashes = 6;
	public float delay = 0.05f;
	public bool disable = false;
	public float flashColor2 = 0.85f;
	private SpriteRenderer[] sprites;

	void Awake()
	{
		sprites = GetComponentsInChildren<SpriteRenderer>();
	}

	void FixedUpdate()
	{
		StartCoroutine(FlashSprites(sprites, flashes, delay, disable));
	}

	public IEnumerator FlashSprites(SpriteRenderer[] sprites, int numTimes, float delay, bool disable) {
		// number of times to loop
		for (int loop = 0; loop < numTimes; loop++) {            
			// cycle through all sprites
			for (int i = 0; i < sprites.Length; i++) {                
				if (disable) {
					// for disabling
					sprites[i].enabled = false;
				} else {
					// for changing the alpha
					sprites[i].color = new Color(sprites[i].color.r, sprites[i].color.g, sprites[i].color.b, flashColor2);
				}
			}
			
			// delay specified amount
			yield return new WaitForSeconds(delay);
			
			// cycle through all sprites
			for (int i = 0; i < sprites.Length; i++) {
				if (disable) {
					// for disabling
					sprites[i].enabled = true;
				} else {
					// for changing the alpha
					sprites[i].color = new Color(sprites[i].color.r, sprites[i].color.g, sprites[i].color.b, 1);
				}
			}
			
			// delay specified amount
			yield return new WaitForSeconds(delay);
		}
	}
}
