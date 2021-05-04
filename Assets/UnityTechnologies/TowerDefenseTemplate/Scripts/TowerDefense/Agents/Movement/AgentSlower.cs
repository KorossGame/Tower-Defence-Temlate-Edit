using System.Collections.Generic;
using Core.Health;
using Core.Utilities;
using UnityEngine;

namespace TowerDefense.Agents
{
	/// <summary>
	/// This effect will get attached to an agent that is within range of the SlowAffector radius
	/// </summary>
	public class AgentSlower : AgentEffect
	{
		protected GameObject m_SlowFx;

		protected List<float> m_CurrentEffects = new List<float>();

		/// <summary>
		/// Initializes the slower with the parameters configured in the SlowAffector
		/// </summary>
		/// <param name="slowFactor">Normalized float that represents the % slowdown applied to the agent</param>
		/// <param name="slowfxPrefab">The instantiated object to visualize the slow effect</param>
		/// <param name="position"></param>
		/// <param name="scale"></param>
		public virtual void Initialize(float slowFactor, GameObject slowfxPrefab = null, 
		                       Vector3 position = default(Vector3),
		                       float scale = 1, bool positiveMode = true)
		{
			LazyLoad();
			m_CurrentEffects.Add(slowFactor);

			float originalSpeed = m_Agent.originalMovementSpeed;
			float newSpeed = 0;

			// Slow mode or speed up
			if (positiveMode)
            {
				// find greatest slow effect
				float min = slowFactor;
				foreach (float item in m_CurrentEffects)
				{
					min = Mathf.Min(min, item);
				}
				newSpeed = originalSpeed * min;
			}
			else
            {
				// find greatest speed up effect
				float max = slowFactor;
				foreach (float item in m_CurrentEffects)
				{
					max = Mathf.Max(max, item);
				}
				newSpeed = originalSpeed * max;
			}
			
			m_Agent.navMeshNavMeshAgent.speed = newSpeed;

			if (m_SlowFx == null && slowfxPrefab != null)
			{
				m_SlowFx = Poolable.TryGetPoolable(slowfxPrefab);
				m_SlowFx.transform.parent = transform;
				m_SlowFx.transform.localPosition = position;
				m_SlowFx.transform.localScale *= scale;
			}

			m_Agent.removed += OnRemoved;
		}

		/// <summary>
		/// Resets the agent's speed 
		/// </summary>
		public void RemoveSlow(float slowFactor)
		{
			m_Agent.removed -= OnRemoved;
			
			m_CurrentEffects.Remove(slowFactor);
			if (m_CurrentEffects.Count != 0)
			{
				return;
			}
			
			// No more slow effects
			ResetAgent();
		}

		/// <summary>
		/// Agent has died, remove affect
		/// </summary>
		protected void OnRemoved(DamageableBehaviour targetable)
		{
			m_Agent.removed -= OnRemoved;
			ResetAgent();
		}

		protected void ResetAgent()
		{
			if (m_Agent != null)
			{
				m_Agent.navMeshNavMeshAgent.speed = m_Agent.originalMovementSpeed;
			}
			if (m_SlowFx != null)
			{
				Poolable.TryPool(m_SlowFx);
				m_SlowFx.transform.localScale = Vector3.one;
				m_SlowFx = null;
			}
			Destroy(this);
		}
	}
}