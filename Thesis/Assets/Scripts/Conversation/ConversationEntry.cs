using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ConversationEntry {
    public string SpeakingCharacterName;
    public string ConversationText;
	public AudioClip sentenceSound;
    public Sprite DisplayPic;
}
