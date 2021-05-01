using System;
using Bloops.GridFramework.Managers;
using Bloops.StateMachine;
using UnityEngine;

namespace Bloops.GridFramework.Items
{
	public class CollectCondition : Collectible, ICondition
	{
		private bool _collected = false;
		[SerializeField] private Transition _transition;
		public override void ItemInitiation()
		{
			base.ItemInitiation();
			_transition.AddCondition(this);
		}
		private void OnDestroy()
		{
			_transition.RemoveCondition(this);
		}
		

		public override void DoCollect()
		{
			_collected = true;
			base.DoCollect();
		}

		public override void UnCollect()
		{
			_collected = false;
			base.UnCollect();
		}

		public bool GetCondition()
		{
			return _collected;
		}
	}
}