using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//require audiosource to play sound
//[RequireComponent(typeof (AudioSource))]

public class ConversationManager : MonoBehaviour {

    //Is there a converastion going on
	[HideInInspector]
    public bool talking;

	//the dialog object
	[SerializeField, HideInInspector]
	public GameObject _DialogBox;
	private MusicFader _fader;
	private AudioClip _sound;
	private AudioClip _bgMusic;
	private float conversationSpeed = .1f;
	private bool submit;
	private PlayerInput _input;

	[Header("UI References")]
	public CanvasGroup _dialogBoxCG;
	public Text _dialog;
	public Text _name;
	public Image _portrait;

	void Start()
	{
		_input = GameObject.Find ("_LevelManager").GetComponent<PlayerInput> ();
		_DialogBox = GameObject.Find ("Dialog");
		_fader = GameObject.Find("Music").GetComponent<MusicFader>();

		if(_DialogBox)
		{
			//these should be blank if no conversation exists
			_dialog.text = null;
			_name.text = null;
		}
	}

	public void PlayMusic(AudioClip music)
	{
		StartCoroutine (Music(music));
	}

	IEnumerator Music(AudioClip music)
	{
		if(_fader)
		{
			//fade in
			_fader.FadeTo(music);

			//loop in here until finished talking,
			while(talking)
			{
				yield return null;
			}
			//fade out
			_fader.FadeTo(_fader.forestTheme);
		}
	}

	//plays the sound attached to the current sentence
	IEnumerator PlaySound(AudioClip sound)
	{
		GetComponent<AudioSource>().PlayOneShot (sound);
		yield return new WaitForSeconds(conversationSpeed);
	}

    public void StartConversation(Conversation conversation)
    {
        //Start displaying the supplied conversation
        if (!talking)
        {
            StartCoroutine(DisplayConversation(conversation));
        }
    }

	//while the button is not pressed, stuck in a loop
	IEnumerator WaitForButton()
	{
		while (!_input._submit)
			yield return null;
	}

    IEnumerator DisplayConversation(Conversation conversation)
    {
		_dialogBoxCG.alpha = 1;
        talking = true;
        foreach (var conversationLine in conversation.ConversationLines)
        {
			//assign current string of dialog to screen
			_dialog.text = conversationLine.ConversationText;
			_name.text = conversationLine.SpeakingCharacterName;
			_portrait.sprite = conversationLine.DisplayPic;
			_sound = conversationLine.sentenceSound;

			StartCoroutine(PlaySound(_sound));
			//wait for button press to resume
			yield return StartCoroutine(WaitForButton());
			yield return new WaitForSeconds(conversationSpeed);
        }

		_dialogBoxCG.alpha = 0;
		_dialog.text = null;
		_name.text = null;

        talking = false;
        yield return null;
    }
}


