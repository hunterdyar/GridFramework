using System.Collections.Generic;
using Bloops.GridFramework.Agents;
using Bloops.GridFramework.Navigation;
using UnityEngine;
using Object = System.Object;

namespace Bloops.GridFramework.Commands
{
	public class SubMove
	{
		public bool finishedExecuting;
		public AgentBase agent;//for reference
		public NavNode startingNode;//for undo
		public object agentStartingData;
		private object agentExtraData;
		public NavNode destinationNode;
		public SubMove criticalFor;
		public List<SubMove> reliesOn = new List<SubMove>();
		public List<AgentBase> reliesOnAgentMoving = new List<AgentBase>();
		public Vector3Int dir;
		private bool moveCritical;
		public int subMovePriority;
		public bool Critical => IsCritical();
		private bool _valid = true;
		public bool Valid => IsValid();
		public SubMove(AgentBase agentBase, NavNode startingNode, object agentStartingData, NavNode destinationNode, Vector3Int dir, bool moveCritical = true,SubMove criticalFor = null,SubMove reliesOn = null, int subMovePriority = 0)
		{
			this.agentStartingData = agentStartingData;
			this.subMovePriority = subMovePriority;
			this.criticalFor = criticalFor;
			SetReliesOn(reliesOn);
			finishedExecuting = false;//todo change this language to finishedAnimation or something more clear, inconsistent use of the word executing.
			this.moveCritical = moveCritical;
			this.dir = dir;//uh, we can just calculate this from destination-starting? I wont jic teleportation becomes like, a move? so we would pass in the previous ... direction entering the teleporter?
			this.agent = agentBase;
			this.startingNode = startingNode;
			this.destinationNode = destinationNode;
		}

		bool IsValid()
		{
			//
			return _valid;
		}

		bool IsCritical()
		{
			return moveCritical;
		}
		public void SetMoveCritical(bool critical)
		{
			moveCritical = critical;
		}

		public void Finish()
		{
			SetFinished(true);
		}

		//Note that we dont have a "revalidate" function. Not even a chance for that problem.
		public void Invalidate()
		{
			_valid = false;
			//no animation needs to play for an invalid move, so lets call it finished with animation.
			finishedExecuting = true;
			
			//should we invalidate the previous move when this move is invalid?
			criticalFor?.Invalidate();

			if (Critical)
			{
				
			}
		}
		public void SetFinished(bool isFinished)
		{
			finishedExecuting = isFinished;
		}

		public void SetReliesOn(SubMove subMove)
		{
			if (subMove != null)
			{
				reliesOn.Add(subMove);
			}
		}

		public bool AllReliesOnIsValid()
		{
			foreach (SubMove r in reliesOn)
			{
				if (!r.Valid)
				{
					return false;
				}
			}

			return true;
		}

		public void ReliesOnAgentMoving(AgentBase agent)
		{
			if (!reliesOnAgentMoving.Contains(agent))
			{
				reliesOnAgentMoving.Add(agent);
			}
		}

		public void SetExtraData(object extraData)
		{
			agentExtraData = extraData;
		}
		public object GetExtraData()
		{
			return agentExtraData;
		}
	}
}