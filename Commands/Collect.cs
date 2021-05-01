using Bloops.GridFramework.Items;
using UnityEngine.Assertions;

namespace Bloops.GridFramework.Commands
{
	public class Collect : ICommand
	{
		public bool HistoryPoint {
			get { return false; }//its always false, this command is run by the game in response to the player landing on things.
		}

		public bool IsValid { get; }

		private Collectible _collectible;

		public Collect(Collectible collectible)
		{
			_collectible = collectible;
			Assert.IsTrue(_collectible.isActiveAndEnabled);
			IsValid = _collectible.CanBeCollected();
		}

		public void Execute()
		{
			//todo, we can let the collectible object handle what it does here. So it can play animations and stuff.
			_collectible.DoCollect();
		}

		public void Undo()
		{
			_collectible.UnCollect();
		}

		public void Redo()
		{
			_collectible.DoCollect();
		}
	}
}