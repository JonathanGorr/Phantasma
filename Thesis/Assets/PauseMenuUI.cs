using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenuUI : UIHandler, ISelectHandler {

	public delegate void Selected();
	public static event Selected selected;

	public override void OnSelect(BaseEventData eventData)
	{
		base.OnSelect(eventData);
		if(selected != null) selected();
	}
}
