using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuSlider : PauseMenuUI {

	public PauseMenu _menu;
	public LevelManager _manager;
	public PlayerInput _input;
	public bool selected = false;
	public Slider slider;
	public float speed = .1f;

	void Update()
	{
		if(!_manager.paused) return;
		if(!selected) return;

		//move the slider value with horizontal axis
		if(Mathf.Abs(_input._axisHorizontal) > 0.1f)
		{
			slider.value += _input._axisHorizontal * speed;
		}
	}

	public override void OnSelect(UnityEngine.EventSystems.BaseEventData eventData)
	{
		base.OnSelect(eventData);
		_menu.canMove = false;
		selected = true;
	}

	public override void OnDeselect(UnityEngine.EventSystems.BaseEventData eventData)
	{
		_menu.canMove = true;
		selected = false;
	}
}
