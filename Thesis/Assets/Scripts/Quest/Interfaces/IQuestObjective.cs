using System.Collections;
using System.Collections.Generic;

namespace QuestSystem
{
	public interface IQuestObjective 
	{
		string Key { get; }
		string Title {get;}
		string Description {get;}
		bool IsActive {get;}
		bool IsComplete {get;}
		void SetComplete();
		void Fail();
		bool IsBonus {get; }
		void Activate();
		void Deactivate();
		void CheckProgress();
	}
}
