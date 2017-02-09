using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// UI:
/// This controls the screen-space UI
/// </summary>

public class UI : MonoBehaviour {

	public LevelManager _manager;
	public QuestSystem.QuestManager _questManager;
	public PlayerInput _input;
	public SFX _sfx;
	public PauseMenu _menu;

	[Header("UI Refs")]
	public CanvasGroup _pauseScreen;
	public CanvasGroup  _controlScreen;
	public CanvasGroup  _ui;
	public CanvasGroup  _dialog;

	[Header("Player")]
	public Slider _healthBar;
	public Slider _staminaBar;
	public CanvasGroup _barsCG;
	public Animator _bloodMeter;

	[Header("Quests")]
	public RectTransform _objectiveUIPrefab;
	public RectTransform _contentGroup;
	public Animator _envelope;
	public Scrollbar _questScrollBar;
	public Text _questUpdates;
	int questUpdates = 0;
	public float questScrollSpeed = .1f;

	public float fadeSpeed = 4;

	//the quest objective list
	//public List<QuestSystem.QuestUI> questUIList = new List<QuestSystem.QuestUI>();
	public List<QuestSystem.ObjectiveUI> objectiveUIList = new List<QuestSystem.ObjectiveUI>();

	void Awake()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
		QuestUpdate(0);
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode m)
	{
		ClearObjectives();

		if(scene.name == "Menu")
		{
			Hide(_barsCG);
		}
		else if(scene.name == "Start")
		{
			Reveal(_barsCG);
		}
	}

	void Update()
	{
		if(_manager.paused)
		{
			_questScrollBar.value += _input._axisVertical * questScrollSpeed;
		}
	}

	public void UpdateHealthBar(int health, int maxHealth)
	{
		_healthBar.maxValue = maxHealth;
		_healthBar.value = health;
	}
	public void UpdateStaminaBar(float stamina, float maxStamina)
	{
		_staminaBar.maxValue = maxStamina;
		_staminaBar.value = stamina;
	}
	public static void TurnOff(CanvasGroup cg)
	{
		cg.alpha = 0;
		cg.interactable = false;
		cg.blocksRaycasts = false;
	}
	public static void TurnOn(CanvasGroup cg)
	{
		cg.alpha = 1;
		cg.interactable = true;
		cg.blocksRaycasts = true;
	}
	public void Hide(CanvasGroup cg)
	{
		StartCoroutine(CGFader(cg, false));
	}
	public void Reveal(CanvasGroup cg)
	{
		StartCoroutine(CGFader(cg, true));
	}
	public void BloodAnim()
	{
		_bloodMeter.SetTrigger("Add");
	}

	//creates a new UI and returns it to the requesting objective to store
	public QuestSystem.ObjectiveUI GetObjectiveUI()
	{
		//create a new objectiveUI prefab
		GameObject o = Instantiate(_objectiveUIPrefab.gameObject, _contentGroup.transform);
		//get its component
		QuestSystem.ObjectiveUI obj = o.GetComponent<QuestSystem.ObjectiveUI>();
		//add it to the list
		objectiveUIList.Add(obj);
		//reset scale because it changes for some reason...
		o.GetComponent<RectTransform>().localScale = Vector3.one;
		//print("created new objective UI for: " + obj.ToString());
		return obj;
	}

	//list clearing
	void ClearObjectives()
	{
		//destroy all objectives
		for(int i=0;i<objectiveUIList.Count;i++)
		{
			Destroy(objectiveUIList[i].gameObject);
		}
		//clear list
		objectiveUIList.Clear();
	}

	//list sorting
	void SortByQuest()
	{
		//TODO: sort by quest to which objectives belong in suborder of completion
	}
	void SortByLevel()
	{
		//TODO: sort by difficulty
	}
	void SortByCompletion()
	{
		//TODO: sort objective list by completion status
	}

	//push update notifications
	public void QuestUpdate(int count)
	{
		questUpdates += count;

		//don't add if already paused
		if(_manager.paused)
		{
			questUpdates = 0;
			_envelope.SetInteger("AnimState", 0);
		}
		//add and notify if paused
		else
		{
			if(questUpdates > 0)
			{
				_envelope.SetInteger("AnimState", 1);
			}
		}

		_questUpdates.text = questUpdates.ToString();
	}

	public void ClearUpdates()
	{
		questUpdates = 0;
		QuestUpdate(0);
	}

	IEnumerator CGFader(CanvasGroup cg, bool reveal)
	{
		//turn off interaction immediately if hiding
		if(!reveal)
		{
			cg.interactable = false;
			cg.blocksRaycasts = false;
		}
		//lerp
		float t = reveal ? 0 : 1;
		while((t < 1 && reveal) || (t > 0 && !reveal))
		{
			t += (reveal ? Time.deltaTime : -Time.deltaTime) * fadeSpeed;
			cg.alpha = t;
			yield return null;
		}
		//turn on interaction at the end of the fade if revealing
		if(reveal)
		{
			cg.interactable = true;
			cg.blocksRaycasts = true;
		}
	}
}
