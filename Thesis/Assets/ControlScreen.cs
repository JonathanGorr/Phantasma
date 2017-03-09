using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlScreen : MonoBehaviour {

	bool shown = false;
	public CanvasGroup controllerCG;
	public CanvasGroup keyboardCG;

	void Awake()
	{
		PlayerInput.onSelect += Toggle;
		PauseMenu.pause += TurnOff;
	}

	void TurnOn()
	{
		if(PlayerInput.Instance.inputState == PlayerInput.InputState.MouseKeyboard) return;
		if(shown) return;
		shown = true;
		Utilities.Instance.Hide(controllerCG);
	}

	void TurnOff()
	{
		if(!shown) return;
		shown = false;
		Utilities.Instance.Reveal(controllerCG);
	}

	void Toggle()
	{
		shown = !shown;
		if(shown)Utilities.Instance.Reveal(controllerCG);
		else Utilities.Instance.Hide(controllerCG);
	}

	void OnDisable()
	{
		PlayerInput.onSelect -= Toggle;
	}
}
