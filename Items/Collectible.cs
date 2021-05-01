using Bloops.GridFramework.Commands;

namespace Bloops.GridFramework.Items
{
	
	public class Collectible : ItemBase
	{
		public void Collect()
		{
			Collect c = new Collect(this);
			puzzleManager.CommandManager.ExecuteCommand(c);
		}

		void ExitAgent()
		{
			
		}

		/// <summary>
		/// Override this to do prerequisite checks, or something like that. Defaults will just return true.
		/// </summary>
		public virtual bool CanBeCollected()
		{
			return true;
		}

		/// <summary>
		/// After the collect command comes through (We created it here, when we called collect -- or when the collection gets undo/redo'd. For playing animations and sounds.
		/// </summary>
		public virtual void DoCollect()
		{
			gameObject.SetActive(false);
		}

		public virtual void UnCollect()
		{
			gameObject.SetActive(true);
		}
	}
}