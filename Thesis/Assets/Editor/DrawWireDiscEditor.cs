using UnityEngine;
using UnityEditor;

[CustomEditor( typeof( DrawWireDisc ) )]
public class DrawWireDiscEditor : Editor
{
	void OnSceneGUI( )
	{
		DrawWireDisc t = target as DrawWireDisc;

		Handles.color = Color.white;
		Handles.Label( t.transform.position + Vector3.up, 
			"ShieldArea: " + t.diameter.ToString( ) );

		Handles.color = t.color;
		Handles.DrawWireDisc( t.transform.position, t.transform.forward, t.diameter );
		
		Handles.color = Color.white;
		t.diameter = Handles.ScaleValueHandle( t.diameter,
			t.transform.position + t.transform.forward * t.diameter,
			t.transform.rotation, 1, Handles.ConeCap, 1 );
	}
}