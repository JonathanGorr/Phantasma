
namespace QuestSystem 
{
	public interface IQuest
	{
		string Key { get; }
		string Title { get; }
		string Description { get; }
		string Hint { get; }
		bool IsActive { get; }
		void SetComplete();
		void Fail();
		void Activate();
		void Deactivate();
		bool CheckProgress();
	}
}