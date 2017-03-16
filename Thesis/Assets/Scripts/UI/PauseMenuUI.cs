using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenuUI : UIHandler, ISelectHandler {

	public LevelManager _manager;
	public delegate void Selected();
	public static event Selected selected;

	public override void Start()
	{
		base.Start();
		if(!_manager) _manager = GameObject.Find("_LevelManager").GetComponent<LevelManager>();
	}

	public override void OnSelect(BaseEventData eventData)
	{
		if(!PauseMenu.paused) return;
		base.OnSelect(eventData);
		if(selected != null) selected();
	}
}
