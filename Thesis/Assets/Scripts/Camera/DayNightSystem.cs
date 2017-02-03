using UnityEngine;
using System.Collections;

public class DayNightSystem : MonoBehaviour {

	public Color[] Colors;
	public Color CastleColor;
	public Color CemetaryColor;
	private Color currentColor;
	private int colorIndex = 0;
	public float transitionSpeed = 0.01f;
	private bool inAVolume;
	private Color nextColor;
	
	void Start()
	{
		Camera.main.backgroundColor = currentColor;
		if (Colors.Length > 0)
		{
			currentColor = Colors[0];
		}
	}
	
	void FixedUpdate()
	{
		for (int i = 0; i < Colors.Length; i++)
		{
			// Get the currentColor in the Array
			if (currentColor == Colors[i])
			{
				colorIndex = i + 1 == Colors.Length ? 0 : i + 1;
			}
		}

		if(!inAVolume)
		{
			nextColor = Colors[colorIndex];
		}

		// Lerp Color _>
		currentColor = Color.Lerp(currentColor, nextColor, transitionSpeed);
		//change the Skybox color
		Camera.main.backgroundColor = currentColor;
		//assign the ambient color to the current
		RenderSettings.ambientLight = currentColor;
	}

	public void Castle()
	{
		inAVolume = true;
		nextColor = CastleColor;
		print ("in castle");
	}

	public void Cemetary()
	{
		inAVolume = true;
		nextColor = CemetaryColor;
		print ("in cemetary");
	}

	public void Outside()
	{
		inAVolume = false;
	}
}
