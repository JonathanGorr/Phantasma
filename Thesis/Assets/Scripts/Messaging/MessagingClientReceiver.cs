using UnityEngine;

public class MessagingClientReceiver : MonoBehaviour
{
	public ConversationManager _conversationManager;
	public MessagingManager _messagingManager;

	void Awake()
	{
		_conversationManager = GameObject.Find("_LevelManager").GetComponent<ConversationManager>();
		_messagingManager = GameObject.Find("_LevelManager").GetComponent<MessagingManager>();
	}

    void Start()
    {
        MessagingManager.Instance.Subscribe(ThePlayerIsTryingToLeave);
    }

    void ThePlayerIsTryingToLeave()
    {
        var dialog = GetComponent<ConversationComponent>();
        if (dialog != null)
        {
            if (dialog.Conversations != null && dialog.Conversations.Length > 0)
            {
                var conversation = dialog.Conversations[0];
                if (conversation != null)
                {
					_conversationManager.StartConversation(conversation);
                }
            }
        }
    }

    void OnDestroy()
    {
        if (MessagingManager.Instance != null)
        {
            MessagingManager.Instance.UnSubscribe(ThePlayerIsTryingToLeave);
        }
    }
}
