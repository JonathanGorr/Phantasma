using UnityEngine;
using UnityEditor;

public class ConversationAssetCreator {

	[MenuItem("Assets/Create/Conversation")]
	public static void CreateConversation()
	{
		CustomAssetUtility.CreateAsset<Conversation>("Assets/Resources/Conversations");
	}

	[MenuItem("Assets/Create/Quest Location Objective")]
	public static void CreateLocationObjective()
	{
		CustomAssetUtility.CreateAsset<QuestSystem.LocationObjective>("Assets/Resources/Quests");
	}

	[MenuItem("Assets/Create/Quest Collection Objective")]
	public static void CreateCollectionObjective()
	{
		CustomAssetUtility.CreateAsset<QuestSystem.CollectionObjective>("Assets/Resources/Quests");
	}

	[MenuItem("Assets/Create/Quest Assassinate Objective")]
	public static void CreateAssassinateObjective()
	{
		CustomAssetUtility.CreateAsset<QuestSystem.AssassinateObjective>("Assets/Resources/Quests");
	}
}
