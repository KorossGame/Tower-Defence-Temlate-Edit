using ActionGameFramework.Health;
using Core.Health;
using Core.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using TowerDefense.Affectors;
using UnityEngine;

namespace TowerDefense.Agents
{
    public class AgentDamageUp : AgentEffect
    {
		protected GameObject m_DamageFx;

		protected List<float> m_CurrentEffects = new List<float>();

		/// <summary>
		/// Initializes the damager with the parameters configured in the DamagaAffector
		/// </summary>
		/// <param name="damageFactor">Normalized float that represents the % slowdown applied to the agent</param>
		/// <param name="damagefxPrefab">The instantiated object to visualize the slow effect</param>
		/// <param name="position"></param>
		/// <param name="scale"></param>
		public virtual void Initialize(float damageFactor, GameObject damagefxPrefab = null,
							   Vector3 position = default(Vector3),
							   float scale = 1, bool positiveMode = true)
		{
			LazyLoad();
			m_CurrentEffects.Add(damageFactor);

			Damager towerDamage;

			try
            {
				towerDamage = m_Tower.GetComponentInChildren<AttackAffector>().projectile.GetComponent<Damager>();
			}
			catch (NullReferenceException)
            {
				return;
            }

			// Behaviour.
			float originalDamage = towerDamage.damage;
			m_Tower.originalDamage = originalDamage;
			float newDamage = 0;

			// Damage up or down
			if (positiveMode)
            {
				// Damage up
				float max = damageFactor;
				foreach (float item in m_CurrentEffects)
				{
					max = Mathf.Max(max, item);
				}
				newDamage = originalDamage * max;
			}
			else
            {
				// Damage down
				float min = damageFactor;
				foreach (float item in m_CurrentEffects)
				{
					min = Mathf.Min(min, item);
				}
				newDamage = originalDamage * min;
			}

			towerDamage.SetDamage(newDamage);

			// Visuals.
			if (m_DamageFx == null && damagefxPrefab != null)
			{
				m_DamageFx = Poolable.TryGetPoolable(damagefxPrefab);
				m_DamageFx.transform.parent = transform;
				m_DamageFx.transform.localPosition = position;
				m_DamageFx.transform.localScale *= scale;
			}

			m_Tower.removed += OnRemoved;
		}

		/// <summary>
		/// Resets the agent's speed 
		/// </summary>
		public void RemoveDamage(float slowFactor)
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
			m_Tower.removed -= OnRemoved;
			ResetAgent();
		}

		protected void ResetAgent()
		{
			if (m_Tower != null)
			{
				m_Tower.gameObject.GetComponentInChildren<AttackAffector>().projectile.GetComponent<Damager>().SetDamage(m_Tower.originalDamage);
			}
			if (m_DamageFx != null)
			{
				Poolable.TryPool(m_DamageFx);
				m_DamageFx.transform.localScale = Vector3.one;
				m_DamageFx = null;
			}
			Destroy(this);
		}
	}
}
