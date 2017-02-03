using UnityEngine;
using System.Collections;

public class ShowDialog : MonoBehaviour {

	//the dialog object
	//must serialize or else reference defaults to null
	[SerializeField, HideInInspector] public GameObject _DialogBox;
	private Transform _target;
	public float convoDistance = 2;
	public bool talked;
	private PlayerInput _input;
	private Conversation conversation;

	private ConversationManager _conversationManager;

	void Start()
	{
		_conversationManager = GameObject.Find("_LevelManager").GetComponent<ConversationManager>();
		_input = GameObject.Find ("_LevelManager").GetComponent<PlayerInput> ();
		_target = GameObject.Find ("_Player").transform;
		_DialogBox = GameObject.Find ("Dialog");
	}

	void Update()
	{
		float distance = Vector3.Distance (transform.position, _target.position);

		if(!talked)
		{
			if(distance < convoDistance)
			{
				if(_DialogBox)
				{
					_DialogBox.SetActive (true);
					var dialog = GetComponent<ConversationComponent>();
					
					//if there is text in the dialog
					if (dialog)
					{
						if (dialog.Conversations != null && dialog.Conversations.Length > 0)
						{
							//what the character says is determined by the control scheme
							if(!_input._controller)
								conversation = dialog.Conversations[0];
							else
								conversation = dialog.Conversations[1];

							//get music in conversation
							var bgMusic = conversation.bgMusic;

							if (conversation)
							{
								_conversationManager.StartConversation(conversation);
								//play music in conversation manager
								_conversationManager.PlayMusic(bgMusic);
								talked = true;
							}
						}
					}
				}
			}
		}
	}
}
