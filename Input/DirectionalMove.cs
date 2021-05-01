using Bloops.GridFramework.Agents;
using Bloops.StateMachine;
using UnityEngine;

namespace Bloops.GridFramework.Input
{
	public class DirectionalMove : MonoBehaviour
	{
		private Player _player;
		[SerializeField] private State dependantOnState;

		void Awake()
		{
			//todo replace with proper data(SO), direct component, or singleton pattern.
			_player = GetComponent<Player>();
		}
		void Update()
		{
			if (dependantOnState != null)
			{
				if (!dependantOnState.IsActive)
				{
					return;
				}
			}

			//todo lets use a proper input manager. Input system or recode, or new input system?
			if (UnityEngine.Input.GetKeyDown(KeyCode.UpArrow)|| UnityEngine.Input.GetKeyDown(KeyCode.W))
			{
				_player.Move(Vector2Int.up);
			}else if (UnityEngine.Input.GetKeyDown(KeyCode.RightArrow)|| UnityEngine.Input.GetKeyDown(KeyCode.D))
			{
				_player.Move(Vector2Int.right);
			}else if (UnityEngine.Input.GetKeyDown(KeyCode.DownArrow)|| UnityEngine.Input.GetKeyDown(KeyCode.S))
			{
				_player.Move(Vector2Int.down);
			}else if (UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow) || UnityEngine.Input.GetKeyDown(KeyCode.A))
			{
				_player.Move(Vector2Int.left);
			}
			//
		
		
		}
	}
}
