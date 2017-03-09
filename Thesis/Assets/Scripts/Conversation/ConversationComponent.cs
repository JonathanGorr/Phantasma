using UnityEngine;

public class ConversationComponent : MonoBehaviour {

    public Conversation[] Conversations;

    void Awake()
    {
    	//replace all with a clone so that the editor versions aren't altered
    	for(int i=0;i<Conversations.Length;i++)
    	{
    		if(Conversations[i] == null) continue;
			Conversations[i] = ScriptableObject.Instantiate(Conversations[i]) as Conversation;
		}
    }

	void OnTriggerEnter2D(Collider2D col)
	{
		if(Conversations.Length == 0) return;

		if(col.CompareTag("Player"))
		{
			//get an unfinished conversation if available
			Conversation c = UncompleteDialog();
			if(c == null) return;

			ConversationManager.Instance.StartConversation(transform, Conversations[0]);
		}
	}

	//returns a conversation that hasn't been yet finished
	Conversation UncompleteDialog()
	{
		for(int i=0; i<Conversations.Length;i++)
		{
			if(!Conversations[i].complete) return Conversations[i];
		}
		return null;
	}
}
