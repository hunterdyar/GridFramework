namespace Bloops.GridFramework.Managers
{
	public class TurnCounter : ICondition
	{
		int _maxTurns = -1;
		private int _turns = 0;
		
		
		public int Turns => _turns;

		public bool GetCondition()
		{
			if (_maxTurns == -1)
			{
				return true;
			}

			return _turns <= _maxTurns;
		}

		public void TakeTurn()
		{
			_turns++;
		}

		public void RemoveTurn()
		{
			_turns--;
		}

		public void SetMaxTurns(int maxTurnsToWinPuzzle)
		{
			_maxTurns = maxTurnsToWinPuzzle;
		}
	}
}