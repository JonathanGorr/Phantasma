using UnityEngine;
using System.Collections;

public class Fading : MonoBehaviour {
	
	public Texture2D fadeOutTexture; //overlay texture
	public float fadeSpeed = 0.8f;	 //fading speed

	private int drawDepth = -1000; 	//make sure its on top of everything else
	private float alpha = 1.0f; 	//texture alpha value
	private int fadeDir = -1; 		//direction to fade: in is -1, out is 1

	void Awake()
	{
		BeginFade (-1); //call the fade in function
	}

	void OnGUI ()
	{
		//fade out/in the alpha value using direction, speed and time.deltaTime to convert the operation to seconds
		alpha += fadeDir * fadeSpeed * Time.deltaTime;
		//force (clamp) the number between 0 and 1 because GUI.color uses alpha values between 0 and 1
		alpha = Mathf.Clamp01 (alpha);

		//set color of our GUI (in this case our texture). All color values remain the same and the alpha is set to the alpha variable
		GUI.color = new Color (GUI.color.r, GUI.color.g, GUI.color.b, alpha);
		GUI.depth = drawDepth;
		GUI.DrawTexture (new Rect (0,0, Screen.width, Screen.height), fadeOutTexture); //draw the texture to fit entire screen
	}

	//set fadedir to the direction parameter making the scene fade in if -1 and out if 1
	public float BeginFade (int direction) 
	{
		fadeDir = direction;
		return (fadeSpeed); //return the fadeSpeed variable so its easy to time the apllication.loadlevel();
	}
}
