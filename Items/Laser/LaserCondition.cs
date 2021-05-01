using System;
using System.Collections.Generic;
using Bloops.GridFramework.Agents;
using Bloops.GridFramework.Managers;
using Bloops.StateMachine;
using UnityEngine;
using UnityEngine.Assertions;

namespace Bloops.GridFramework.Items.Laser
{
	public class LaserCondition : AgentBase, ILaserActivated, ICondition
	{
		protected readonly List<LaserSource> _laserSources = new List<LaserSource>();
		[SerializeField] private Transition _transition;

		protected override void AgentInitiation()
		{
			base.AgentInitiation();
			_transition.AddCondition(this);
		}
		
		public void SetLaserSource(LaserSource source)
		{
			Assert.IsFalse(_laserSources.Contains(source));
			_laserSources.Add(source);
			OnLaserCountChanged(_laserSources.Count);
		}

		public void RemoveLaserSource(LaserSource source)
		{
			//Safe check, because who doesn't clear their laser beams EXCESSIVELY?
			if (_laserSources.Contains(source))
			{
				_laserSources.Remove(source);
				OnLaserCountChanged(_laserSources.Count);
			}

		}

		public bool HasLaserSource(LaserSource source)
		{
			return _laserSources.Contains(source);
		}

		public bool GetCondition()
		{
			return _laserSources.Count > 0;
		}

		/// <summary>
		/// Called when a laser source is added or removed, not neccesarily when it goes from 0 to 1. You may want to change graphics when multiple lasers hit something, require more than 0, or only 1, laser to activate a thing. Who knows.
		/// The base objcet currently changes a sppriteRender color but thats just some temp code for testing. If your're reading this comment, i forgot to remove it and put it in its own class or UnityEvent that can handle graphics.
		/// </summary>
		/// <param name="numLasers">_laserSources.Count is passed along. 0 means no lasers, >0 means lasers.</param>
		protected virtual void OnLaserCountChanged(int numLasers)
		{
			if (numLasers <= 0)
			{
				GetComponent<SpriteRenderer>().color = Color.cyan;
			}
			else
			{
				GetComponent<SpriteRenderer>().color = Color.red;
			}
		}

		protected void OnDestroy()
		{
			_transition.RemoveCondition(this);
		}
	}
}
