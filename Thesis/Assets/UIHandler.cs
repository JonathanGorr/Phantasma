using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour, ISelectHandler, IDeselectHandler {

	[HideInInspector] public SFX _sfx;

	public virtual void Start()
	{
		_sfx = GameObject.Find("_LevelManager").GetComponent<SFX>();
		if(GetComponent<Button>()) GetComponent<Button>().onClick.AddListener(OnPress);
	}

	//on selected for controller scrolling
	public virtual void OnSelect(BaseEventData eventData)
	{
		_sfx.PlayUI("scroll");
	}

	//on deselected for controller scrolling
	public virtual void OnDeselect(BaseEventData eventData)
	{
	}

	void OnPress()
	{
		_sfx.PlayUI("select");
	}
}
