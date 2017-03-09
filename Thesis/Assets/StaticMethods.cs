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
}
