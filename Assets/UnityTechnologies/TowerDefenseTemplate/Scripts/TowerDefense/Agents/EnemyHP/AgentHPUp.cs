using Core.Health;
using Core.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense.Agents
{
	public class AgentHPUp : AgentEffect
	{
		protected GameObject m_HPFx;

		protected List<float> m_CurrentEffects = new List<float>();

		/// <summary>
		/// Initializes the slower with the parameters configured in the SlowAffector
		/// </summary>
		/// <param name="hpFactor">Normalized float that represents the % slowdown applied to the agent</param>
		/// <param name="hpfxPrefab">The instantiated object to visualize the slow effect</param>
		/// <param name="position"></param>
		/// <param name="scale"></param>
		public virtual void Initialize(float hpFactor, GameObject hpfxPrefab = null,
							   Vector3 position = default(Vector3),
							   float scale = 1, bool positiveMode = true)
		{
			LazyLoad();
			m_CurrentEffects.Add(hpFactor);

			// Behaviour.
			float originalHP = m_Agent.configuration.currentHealth;
			m_Agent.originalHPAmount = originalHP;

			float newHP = 0;

			// Enemy HP up or down
			if (positiveMode)
			{
				// Enemy HP down
				float min = hpFactor;
				foreach (float item in m_CurrentEffects)
				{
					min = Mathf.Min(min, item);
				}
				newHP = originalHP * min;
				
			}
			else
			{
				// Enemy HP up
				float max = hpFactor;
				foreach (float item in m_CurrentEffects)
				{
					max = Mathf.Max(max, item);
				}
				newHP = originalHP * max;
			}

			// Apply new HP
			m_Agent.configuration.SetHealth(newHP);

			if (m_HPFx == null && hpfxPrefab != null)
			{
				m_HPFx = Poolable.TryGetPoolable(hpfxPrefab);
				m_HPFx.transform.parent = transform;
				m_HPFx.transform.localPosition = position;
				m_HPFx.transform.localScale *= scale;
			}

			m_Agent.removed += OnRemoved;
		}

		/// <summary>
		/// Resets the agent's speed 
		/// </summary>
		public void RemoveSlow(float hpFactor)
		{
			m_Agent.removed -= OnRemoved;

			m_CurrentEffects.Remove(hpFactor);
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
				m_Agent.configuration.SetHealth(m_Agent.originalHPAmount);
			}
			if (m_HPFx != null)
			{
				Poolable.TryPool(m_HPFx);
				m_HPFx.transform.localScale = Vector3.one;
				m_HPFx = null;
			}
			Destroy(this);
		}
	}

}