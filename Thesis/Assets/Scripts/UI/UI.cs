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

	public static UI Instance = null;

	[Header("UI Refs")]
	public CanvasGroup  cg;
	public CanvasGroup _bloodMeterCG;
	public CanvasGroup _questIcon;
	public Animator cinemaBars;

	[Header("Player")]
	public Slider _healthBar;
	public Slider _staminaBar;
	public CanvasGroup _barsCG;
	public Animator _bloodMeter;

	[Header("Quests")]
	public QuestSystem.QuestManager _questManager;
	public RectTransform _objectiveUIPrefab;
	public RectTransform _contentGroup;
	public Animator _envelope;
	public Scrollbar _questScrollBar;
	public Text _questUpdates;
	int questUpdates = 0;
	public float questScrollSpeed = .1f;

	public float fadeSpeed = 4;

	public Slider MyHealthBar
	{
		get { return _healthBar; }
	}

	public Slider MyStaminaBar
	{
		get { return _staminaBar; }
	}

	//the quest objective list
	//public List<QuestSystem.QuestUI> questUIList = new List<QuestSystem.QuestUI>();
	public List<QuestSystem.ObjectiveUI> objectiveUIList = new List<QuestSystem.ObjectiveUI>();

	void Awake()
	{
		if(Instance == null) Instance = this;
		SceneManager.sceneLoaded += OnSceneLoaded;
		QuestUpdate(0);
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode m)
	{
		ClearObjectives();

		if(scene.name == "Menu")
		{
			_barsCG.TurnOff();
			_bloodMeterCG.TurnOff();
			_questIcon.TurnOff();
		}
		else if(scene.name == "Start")
		{
			_barsCG.TurnOn();
			_bloodMeterCG.TurnOn();
			_questIcon.TurnOn();
		}
	}

	void Update()
	{
		if(PauseMenu.paused)
		{
			_questScrollBar.value += PlayerInput.Instance.RAnalog.y * questScrollSpeed;
		}
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
		if(PauseMenu.paused)
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
}
