using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuSlider : PauseMenuUI {

	public PauseMenu _menu;
	public PlayerInput _input;
	public Slider slider;
	public bool isSelected = false;
	public float speed = .1f;

	void Update()
	{
		if(!_manager.paused) return;
		if(!isSelected) return;

		//move the slider value with horizontal axis
		if(Mathf.Abs(_input.RAnalog.x) > 0.1f)
		{
			slider.value += _input.RAnalog.x * speed;
		}
	}

	public override void OnSelect(UnityEngine.EventSystems.BaseEventData eventData)
	{
		base.OnSelect(eventData);
		_menu.canMove = false;
		isSelected = true;
	}

	public override void OnDeselect(UnityEngine.EventSystems.BaseEventData eventData)
	{
		_menu.canMove = true;
		isSelected = false;
	}
}
