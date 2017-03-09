using UnityEngine;

public class Conversation : ScriptableObject {

	public AudioClip bgMusic;
    public ConversationEntry[] ConversationLines;
    //public Objective objective; //TODO: reimplement
    public bool complete;
}
