using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//require audiosource to play sound
//[RequireComponent(typeof (AudioSource))]

public class ConversationManager : Singleton<ConversationManager> {
    protected ConversationManager() { } // guarantee this will be always a singleton only - can't use the constructor!

    //Is there a converastion going on
	[HideInInspector]
    public bool talking = false;

    //The current line of text being displayed
    ConversationEntry currentConversationLine;

	//the dialog object
	[SerializeField, HideInInspector]
	public GameObject _DialogBox;
	private MusicFader _fader;
	private Text _dialog;
	private Text _name;
	private Image _portrait;
	private AudioClip _sound;
	private AudioClip _bgMusic;
	public float conversationSpeed = 2f;

	void Awake()
	{
		_DialogBox = GameObject.Find ("Dialog");
		_fader = GameObject.Find("Music").GetComponent<MusicFader>();

		if(_DialogBox != null)
		{
			_dialog = GameObject.Find("DialogText").GetComponent<Text>();
			_name = GameObject.Find ("CharacterName").GetComponent<Text>();
			_portrait = GameObject.Find("SpeakerPortrait").GetComponent<Image>();

			//these should be blank if no conversation exists
			_dialog.text = null;
			_name.text = null;
		}
	}

	void FixedUpdate()
	{
		if(_dialog != null)
		{
			//toggle dialog on conversation
			if(talking)
			{
				//assign current string of dialog to screen
				_dialog.text = currentConversationLine.ConversationText;
				_name.text = currentConversationLine.SpeakingCharacterName;
				_portrait.sprite = currentConversationLine.DisplayPic;
				_sound = currentConversationLine.sentenceSound;
			}
			else if(!talking)//hide if not talking
			{
				_dialog.text = null;
				_name.text = null;
				_DialogBox.SetActive (false);
			}
		}
	}

	public void PlayMusic(AudioClip music)
	{
		StartCoroutine (Music(music));
	}

	IEnumerator Music(AudioClip music)
	{
		if(_fader != null)
		{
			//fade in
			_fader.Fade(music);
			//loop in here until finished talking,
			while(talking)
			{
				yield return null;
			}
			//fade out
			_fader.Fade(_fader.forestTheme);
		}
		else
			print("fader cannot be found");
	}

	//plays the sound attached to the current sentence
	IEnumerator PlaySound(AudioClip sound)
	{
		GetComponent<AudioSource>().PlayOneShot (sound);
		yield return new WaitForSeconds(conversationSpeed);
	}

    public void StartConversation(Conversation conversation)
    {
        //Start displying the supplied conversation
        if (!talking)
        {
			_DialogBox.SetActive (true);
            StartCoroutine(DisplayConversation(conversation));
        }
    }

	//while the button is not pressed, stuck in a loop
	IEnumerator WaitForButton()
	{
		while (!Input.GetButtonDown("360_AButton"))
			yield return null;
	}

    IEnumerator DisplayConversation(Conversation conversation)
    {
        talking = true;
        foreach (var conversationLine in conversation.ConversationLines)
        {
            currentConversationLine = conversationLine;
			StartCoroutine(PlaySound(_sound));
			//wait for button press to resume
			yield return StartCoroutine(WaitForButton());
			yield return new WaitForSeconds(conversationSpeed);
        }
        talking = false;
        yield return null;
    }
}


