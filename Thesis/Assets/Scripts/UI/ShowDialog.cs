using UnityEngine;
using System.Collections;

public class ShowDialog : MonoBehaviour {

	//the dialog object
	//must serialize or else reference defaults to null
	[SerializeField, HideInInspector]
	public GameObject _DialogBox;

	void Awake()
	{
		_DialogBox = GameObject.Find ("Dialog");
	}

	//play conversation attached to trigger
	void OnTriggerEnter2D(Collider2D trigger)
	{
		if(_DialogBox != null)
		{
			if(trigger.gameObject.tag == "Player")
			{
				_DialogBox.SetActive (true);
				var dialog = GetComponent<ConversationComponent>();

				//if there is text in the dialog, 
				if (dialog != null)
				{
					if (dialog.Conversations != null && dialog.Conversations.Length > 0)
					{
						var conversation = dialog.Conversations[0];
						//get music in conversation
						var bgMusic = conversation.bgMusic;
						if (conversation != null)
						{
							ConversationManager.Instance.StartConversation(conversation);
							//play music in conversation manager
							ConversationManager.Instance.PlayMusic(bgMusic);
						}
					}
				}
			}
		}
	}
}
