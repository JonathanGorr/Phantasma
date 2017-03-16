using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using RedBlueGames.Tools.TextTyper;
using System.Collections.Generic;
using System;

/*
    scripts.Enqueue("Hi");
    scripts.Enqueue("Hello! My name is<delay=0.5>... </delay>NPC. Got it, bub?");
    scripts.Enqueue("You can <b>use</b> <i>uGUI</i> <size=40>text</size> <size=20>tag</size> and <color=#ff0000ff>color</color> tag <color=#00ff00ff>like this</color>.");
    scripts.Enqueue("bold <b>text</b> test <b>bold</b> text <b>test</b>");
    scripts.Enqueue("You can <size=40>size 40</size> and <size=20>size 20</size>");
    scripts.Enqueue("You can <color=#ff0000ff>color</color> tag <color=#00ff00ff>like this</color>.");
*/

public class ConversationManager : MonoBehaviour {

	private Queue<ConversationEntry> scripts = new Queue<ConversationEntry>();
	[SerializeField]
    [Tooltip("The text typer element to test typing with")]
    private TextTyper testTextTyper;

	public static ConversationManager Instance = null;

    //Is there a converastion going on
	[HideInInspector]
    public bool talking;

	public AudioSource dialogueAsrc;
	public AudioSource letterAsrc;
	public CanvasGroup _DialogBox;
	private AudioClip _sound;
	private AudioClip _bgMusic;
	private bool submit;
	public bool PlayLetterSound = true;
	public AudioClip letterSound;

	[Header("UI References")]
	public Text _dialog;
	public Text _name;
	public Image _portrait;

	ConversationEntry currentConversationEntry;

	public delegate void ConversationStarted();
	public static event ConversationStarted onConversationStarted;

	public delegate void ConversationEnded();
	public static event ConversationEnded onConversationEnded;

	void Awake()
	{
		if(Instance == null) Instance = this;
        this.testTextTyper.CharacterPrinted.AddListener(this.HandleCharacterPrinted);
		this.testTextTyper.PrintCompleted.AddListener(this.HandleDialogueComplete);
		this.testTextTyper.LineBegan.AddListener(this.HandleNewLine); 

		//these should be blank if no conversation exists
		_dialog.text = null;
		_name.text = null;
		PlayerInput.onA += Skip;
	}

    public void StartConversation(Transform speaker, Conversation conversation)
    {
        //Start displaying the supplied conversation
        if (!talking)
        {
			StartCoroutine(DisplayConversation(speaker, conversation));
        }
    }

    public void SetVolume(float v)
    {
    	dialogueAsrc.volume = v;
		letterAsrc.volume = v;
    }

    //used to skip dialog
    void Skip()
    {
		if (this.testTextTyper.IsSkippable())
        {
            this.testTextTyper.Skip();
        }
        else
        {
            ShowScript();
        }
    }

	void ShowScript()
    {
        if (scripts.Count <= 0)
        {
			talking = false;
            return;
        }

        //update the current conversation entry which also dequeues the object
		currentConversationEntry = scripts.Dequeue();
		this.testTextTyper.TypeText(currentConversationEntry);
		this._portrait.sprite = currentConversationEntry.DisplayPic;
		this._name.text = currentConversationEntry.SpeakingCharacterName;
    }

	private void HandleCharacterPrinted(string printedCharacter)
    {
        // Do not play a sound for whitespace
        if (printedCharacter == " " || printedCharacter == "\n")
        {
            return;
        }
        //play sound
		letterAsrc.PlayOneShot(this.letterSound);
    }

    //called whenever a line of dialogue is completed
	void HandleDialogueComplete()
    {
    }

	//called whenever a line of dialogue is started
	void HandleNewLine()
    {
		dialogueAsrc.PlayOneShot(currentConversationEntry.sentenceSound);
    }

	IEnumerator DisplayConversation(Transform speaker, Conversation conversation)
    {
    	if(onConversationStarted != null) onConversationStarted();
    	UI.Instance.cinemaBars.SetBool("On", true);
    	PostProcessingHandler.Instance.ShowVignette();
		CameraController.Instance.RegisterMe(speaker);
		MusicFader.Instance.FadeTo(conversation.bgMusic);
		Utilities.Instance.Reveal(_DialogBox);
        talking = true;

		//enque all lines from conversation
		for(int i=0;i<conversation.ConversationLines.Length; i++)
		{
			//the delay tags are used to fix a delay bug
			scripts.Enqueue(conversation.ConversationLines[i]);//"<delay=0></delay>" + 
		}

		ShowScript();
		while(talking) yield return null; //wait until all lines of dialogue are dequeued

		if(onConversationEnded != null) onConversationEnded();
		UI.Instance.cinemaBars.SetBool("On", false);
		MusicFader.Instance.FadeTo(MusicFader.Instance.forestTheme);
		Utilities.Instance.Hide(_DialogBox);
		_dialog.text = null;
		_name.text = null;
		conversation.complete = true;
		CameraController.Instance.UnRegisterMe(speaker);
		PostProcessingHandler.Instance.HideVignette();
        yield return null;
    }
}


