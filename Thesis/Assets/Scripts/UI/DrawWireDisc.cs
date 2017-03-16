using UnityEngine;
          
[ExecuteInEditMode]
public class DrawWireDisc : MonoBehaviour
{
	public DrawWireDisc(float d, Color c)
	{
		color = c;
		diameter = d;
	}

	[HideInInspector] public Color color = Color.white;
	[HideInInspector] public float diameter = 5;
}