using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum Tabs { Inventory, Quests, Options }

public class PauseMenu : MonoBehaviour {

	[Header("References")]
	public EventSystem _eventSystem;
	public LevelManager _manager;
	public UI _ui;
	public PlayerInput _input;

	[HideInInspector] public bool canMove = true;

	[Header("UI")]
	public CanvasGroup cg;
	public RectTransform myRect;
	public float moveSpeed = .5f;

	[Header("Selection Box")]
	public RectTransform selectionBox;
	private RectTransform currentlySelected;
	public float lerpSpeed = 2;
	public float padding = 1f;

	Rect screenRect = new Rect(0,0, Screen.width, Screen.height);

	[Header("Buttons")]
	public Button restart;
	public Button quit;

	[Header("Tabs")]
	public Tabs currentTab = Tabs.Inventory;
	[Header(" ")]
	public Button inventory;
	public CanvasGroup inventoryCG;
	[Header(" ")]
	public Button quests;
	public CanvasGroup questsCG;
	[Header(" ")]
	public Button options;
	public CanvasGroup optionsCG;

	int tabLength;

	//currently selected GO
	RectTransform Current
	{
		get 
		{
			//if currently selected is null, or currently selected isn't what the eventsytem says is currently selected...
			if(currentlySelected == null || currentlySelected.gameObject != _eventSystem.currentSelectedGameObject)
			{
				//if the eventsystem isn't selecting anything, select default
				if(_eventSystem.currentSelectedGameObject == null)
				{
					_eventSystem.SetSelectedGameObject(Default);
				 	currentlySelected = _eventSystem.currentSelectedGameObject.GetComponent<RectTransform>();
				}
				//if the eventsystem is selecting something, set current to it
				else
				{
					currentlySelected = _eventSystem.currentSelectedGameObject.GetComponent<RectTransform>();
				}
			}
			return currentlySelected;
		}
		set 
		{
			_eventSystem.SetSelectedGameObject(value.gameObject);
		 	currentlySelected = value;
		}
	}

	GameObject Default
	{
		//this is default
		get { return inventory.gameObject; }
	}

	void Awake()
	{
		screenRect = new Rect(0,0, Screen.width, Screen.height);
		LevelManager.pause += OnPause;
		LevelManager.unPause += OnResume;
		PauseMenuUI.selected += OnSelected;
		_input.onL1 += TabLeft;
		_input.onR1 += TabRight;
		//gets length of tabs
		tabLength = System.Enum.GetValues(typeof(Tabs)).Length;
	}

	void Start()
	{
		StartCoroutine(Reset());
		_eventSystem.firstSelectedGameObject = Default;
	}

	void OnDisable()
	{
		LevelManager.pause -= OnPause;
		LevelManager.unPause -= OnResume;
		PauseMenuUI.selected -= OnSelected;
		_input.onL1 -= TabLeft;
		_input.onR1 -= TabRight;
	}

	//sets the current selected uI object to options, resets tab open
	IEnumerator Reset()
	{
		//wait until fully hidden to reset
		while(cg.alpha > 0) yield return null;
		_eventSystem.SetSelectedGameObject(Default);
		Current = _eventSystem.currentSelectedGameObject.GetComponent<RectTransform>();
		SetTab((int)Tabs.Inventory);
		MoveBoxToTab();
	}

	void TabLeft()
	{
		if((int)currentTab == 0) return;
		currentTab = (Tabs)(currentTab - 1);
		SetTab((int)currentTab);
		MoveBoxToTab();
	}

	void TabRight()
	{
		if((int)currentTab == tabLength - 1) return;
		currentTab = (Tabs)currentTab + 1;
		SetTab((int)currentTab);
		MoveBoxToTab();
	}

	void MoveBoxToTab()
	{
		switch(currentTab)
		{
			case Tabs.Inventory:
				Current = inventory.GetComponent<RectTransform>();
				StartCoroutine(MoveBox());
			break;
			case Tabs.Quests:
				Current = quests.GetComponent<RectTransform>();
				StartCoroutine(MoveBox());
			break;
			case Tabs.Options:
				Current = options.GetComponent<RectTransform>();
				StartCoroutine(MoveBox());
			break;
		}
	}

	//called when paused
	void OnPause()
	{
	}

	public void SetTab(int tab)
	{
		//don't bother if not different tab
		currentTab = (Tabs)tab;

		switch(currentTab)
		{
			case Tabs.Inventory:
			UI.TurnOn(inventoryCG);
			UI.TurnOff(questsCG);
			UI.TurnOff(optionsCG);
			break;
			case Tabs.Quests:
			UI.TurnOff(inventoryCG);
			UI.TurnOn(questsCG);
			UI.TurnOff(optionsCG);
			break;
			case Tabs.Options:
			UI.TurnOff(inventoryCG);
			UI.TurnOff(questsCG);
			UI.TurnOn(optionsCG);
			break;
		}
	}

	//called when unpaused
	void OnResume()
	{
		StartCoroutine(Reset());
	}

	//called when a UI button is selected
	public void OnSelected()
	{
		//move the selection box to the new selection
		StartCoroutine(MoveBox());
	}

	//used to smooth move the selection box to the selected object
	IEnumerator MoveBox()
	{
		//get last pos
		Vector2 lastSize = selectionBox.sizeDelta;
		Vector2 lastPos = selectionBox.position;
		float t = 0;
		//lerp to next pos from last
		while(t < 1)
		{
			t += Time.deltaTime * lerpSpeed;
			selectionBox.sizeDelta = Vector2.Lerp(lastSize, Current.sizeDelta + new Vector2(padding, padding), t);
			selectionBox.position = Vector2.Lerp(lastPos, Current.position, t);
			yield return null;
		}
	}

	void Update()
	{
		if(!_manager.paused) return;

		//move around window
		if(canMove)
		{
			if(currentTab != Tabs.Quests)
			{
				if(Mathf.Abs(_input.RAnalog.x) > 0.1f || Mathf.Abs(_input.RAnalog.y) > 0.1f)
				{
					myRect.position = ClampToRect(screenRect, myRect) + (_input.RAnalog * moveSpeed);
					selectionBox.position = Current.position;
				}
			}
		}
	}

	//returns a position clamped
	public static Vector2 ClampToRect(Rect bounds, RectTransform toClamp)
    {
    	//get my pos
		Vector2 pos = toClamp.position;

        //get
		Vector3 minPosition = bounds.min - toClamp.rect.min;
		Vector3 maxPosition = bounds.max - toClamp.rect.max;
 
		pos.x = Mathf.Clamp(toClamp.position.x, minPosition.x, maxPosition.x);
		pos.y = Mathf.Clamp(toClamp.position.y, minPosition.y, maxPosition.y);

		return pos;
    }
}
