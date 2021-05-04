using Core.Health;
using Core.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense.Agents
{
	public class AgentFireRateUp : AgentEffect
	{
		protected GameObject m_FireRateFx;

		protected List<float> m_CurrentEffects = new List<float>();

		/// <summary>
		/// Initializes the slower with the parameters configured in the SlowAffector
		/// </summary>
		/// <param name="fireRateFactor">Normalized float that represents the % slowdown applied to the agent</param>
		/// <param name="ratefxPrefab">The instantiated object to visualize the slow effect</param>
		/// <param name="position"></param>
		/// <param name="scale"></param>
		public virtual void Initialize(float fireRateFactor, GameObject ratefxPrefab = null,
							   Vector3 position = default(Vector3),
							   float scale = 1, bool positiveMode = true)
		{
			LazyLoad();
			m_CurrentEffects.Add(fireRateFactor);

			// Behaviour.
			float originalFireRate = m_Tower.gameObject.GetComponent<AttackingAgent>().m_AttackAffector.fireRate;
			m_Tower.originalFireRateSpeed = originalFireRate;

			float newFireRate = 0;

			// Fire rate up or down
			if (positiveMode)
			{
				// Fire rate up
				float max = fireRateFactor;
				foreach (float item in m_CurrentEffects)
				{
					max = Mathf.Max(max, item);
				}
				newFireRate = fireRateFactor * max;
			}
			else
			{
				// Fire rate down
				float min = fireRateFactor;
				foreach (float item in m_CurrentEffects)
				{
					min = Mathf.Min(min, item);
				}
				newFireRate = fireRateFactor * min;
			}

			// Apply new fire rate
			m_Agent.gameObject.GetComponent<AttackingAgent>().m_AttackAffector.fireRate = newFireRate;

			// Visuals.
			if (m_FireRateFx == null && ratefxPrefab != null)
			{
				m_FireRateFx = Poolable.TryGetPoolable(ratefxPrefab);
				m_FireRateFx.transform.parent = transform;
				m_FireRateFx.transform.localPosition = position;
				m_FireRateFx.transform.localScale *= scale;
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
				m_Tower.gameObject.GetComponent<AttackingAgent>().m_AttackAffector.fireRate = m_Tower.originalFireRateSpeed;
			}
			if (m_FireRateFx != null)
			{
				Poolable.TryPool(m_FireRateFx);
				m_FireRateFx.transform.localScale = Vector3.one;
				m_FireRateFx = null;
			}
			Destroy(this);
		}
	}

}