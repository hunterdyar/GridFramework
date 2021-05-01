namespace Bloops.GridFramework.Commands
{
	public interface ICommand
	{
		/// <summary>
		/// historyPoint are points where, when we press undo, we should stop at.
		/// Any move the player inputs as a single command should probably be true, a history point, while any move that auto-propogates probably should not be a history point.
		/// </summary>
		bool HistoryPoint { get; }
		/// <summary>
		/// Commands may check their given state for validity. Invalid commands wont get executed. They will be ignored.
		/// </summary>
		bool IsValid { get; }
		void Execute();
		
		/// <summary>
		/// Should return the command to its previous state. This should set executed to false.
		/// </summary>
		void Undo();
		/// <summary>
		/// Redo may, in many cases, just call Execute(), but I like my redo's to be instantly snapping, not re-doing sound effects and animation.
		/// Should set executed to true.
		/// </summary>
		void Redo();

	}
}