using UnityEngine;
using System.Collections.Generic;
using System.IO;

public static class StaticMethods {

	public static bool IsInLayerMask(this LayerMask mask, GameObject obj) 
	{
	 return ((mask.value & (1 << obj.layer)) > 0);
	}

	public static void TurnOff(this CanvasGroup cg)
	{
		cg.alpha = 0;
		cg.interactable = false;
		cg.blocksRaycasts = false;
	}
	public static void TurnOn(this CanvasGroup cg)
	{
		cg.alpha = 1;
		cg.interactable = true;
		cg.blocksRaycasts = true;
	}

	public static void LookAtTarget(this Transform t, Transform target)
	{
		Vector3 dir;
		if((target.position - t.position).x > 0)
		{
			dir = (target.position - t.position);
		}
		else
		{
			dir = (t.position - target.position);
		}
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		t.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}

	//retrieves a streaming assets file
	public static string GetStreamingAsset(string fileName)
	{
		string filePath = Application.streamingAssetsPath + "/" + fileName;
        string jsonString;

        if(Application.platform == RuntimePlatform.Android) //Need to extract file from apk first
        {
            WWW reader = new WWW(filePath);
            while (!reader.isDone) { }

            jsonString = reader.text;
        }
        else
        {
            jsonString = File.ReadAllText(filePath);
        }
        
		return jsonString;
	}

	//calculates a physics parabolic trajectory with start, end and time as parameters
	//time determines the height of the parabola
	public static Vector3 calculateBestThrowSpeed(Vector3 origin, Vector3 target, float timeToTarget) 
	{
	     // calculate vectors
	     Vector3 toTarget = target - origin;
	     Vector3 toTargetXZ = toTarget;
	     toTargetXZ.y = 0;
	     
	     // calculate xz and y
	     float y = toTarget.y;
	     float xz = toTargetXZ.magnitude;
	     
	     // calculate starting speeds for xz and y. Physics forumulase deltaX = v0 * t + 1/2 * a * t * t
	     // where a is "-gravity" but only on the y plane, and a is 0 in xz plane.
	     // so xz = v0xz * t => v0xz = xz / t
	     // and y = v0y * t - 1/2 * gravity * t * t => v0y * t = y + 1/2 * gravity * t * t => v0y = y / t + 1/2 * gravity * t
	     float t = timeToTarget;
	     float v0y = y / t + 0.5f * Physics.gravity.magnitude * t;
	     float v0xz = xz / t;
	     
	     // create result vector for calculated starting speeds
	     Vector3 result = toTargetXZ.normalized;        // get direction of xz but with magnitude 1
	     result *= v0xz;                                // set magnitude of xz to v0xz (starting speed in xz plane)
	     result.y = v0y;                                // set y to v0y (starting speed of y plane)
	     
	     return result;
 	}
}
