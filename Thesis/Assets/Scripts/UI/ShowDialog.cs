using UnityEngine;
using System.Collections;

public class ShowDialog : MonoBehaviour {

	//the dialog object
	//must serialize or else reference defaults to null
	[SerializeField, HideInInspector] public GameObject _DialogBox;
	private Transform _target;
	public float convoDistance = 2;
	public bool talked;

	void Awake()
	{
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
							var conversation = dialog.Conversations[0];
							//get music in conversation
							var bgMusic = conversation.bgMusic;

							if (conversation)
							{
								ConversationManager.Instance.StartConversation(conversation);
								//play music in conversation manager
								ConversationManager.Instance.PlayMusic(bgMusic);
								talked = true;
							}
						}
					}
				}
			}
		}
	}
}
