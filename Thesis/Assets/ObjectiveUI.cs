using UnityEngine;
using UnityEngine.UI;

namespace QuestSystem
{
	public class ObjectiveUI : MonoBehaviour {

		private QuestObjective obj; //store a reference to the dictionary entry for this objective rather than a key?
		public Text title;
		public Text description;
		public Toggle completeToggle;
		//rewards
		public RectTransform rewardUIPrefab;
		public RectTransform rewardGroup;

		public QuestObjective Objective
		{
			get { return obj; }
			set { obj = value; }
		}

		public void UpdateUI(QuestObjective qo)
		{
			if(qo == null) print("null");
			obj = qo;
			title.text = qo.Title;
			description.text = qo.Description;
			completeToggle.isOn = qo.isComplete;

			if(qo.rewards.Length == 0) return;

			//add rewards if not already
			if(rewardGroup.childCount < qo.rewards.Length)
			{
				//if rewards already present, delete all
				if(rewardGroup.childCount > 0)
				{
					foreach(RectTransform rt in rewardGroup.transform)
					{
						Destroy(rt);
					}
				}

				//create and assign instantiated rewards icons
				for(int i=0;i<qo.rewards.Length;i++)
				{
					RectTransform r = Instantiate(rewardUIPrefab, rewardGroup) as RectTransform;
					r.localScale = Vector3.one;
					//assign this UI image the inventory sprite for this reward
					r.Find("Item").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Items/" 
					+ qo.rewards[i]);
				}
			}
		}
	}
}
