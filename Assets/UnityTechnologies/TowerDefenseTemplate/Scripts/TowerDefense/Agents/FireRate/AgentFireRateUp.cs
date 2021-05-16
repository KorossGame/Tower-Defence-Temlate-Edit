using System;
using Core.Health;
using Core.Utilities;
using System.Collections;
using System.Collections.Generic;
using ActionGameFramework.Health;
using TowerDefense.Affectors;
using UnityEngine;

namespace TowerDefense.Agents
{
	public class AgentFireRateUp : AgentEffect
	{
		protected GameObject m_FireRateFx;
		public AttackAffector affector;
		
		private float originalFireRate = -1;
		private float oldFireRate = -1;

		private float fireRateFactor;

		protected List<float> m_CurrentEffects = new List<float>();

		/// <summary>
		/// Initializes the slower with the parameters configured in the SlowAffector
		/// </summary>
		/// <param name="fireRateFactor">Normalized float that represents the % slowdown applied to the agent</param>
		/// <param name="ratefxPrefab">The instantiated object to visualize the slow effect</param>
		/// <param name="position"></param>
		/// <param name="scale"></param>
		public virtual void Initialize(float fireRateFactorNew, GameObject ratefxPrefab = null,
							   Vector3 position = default(Vector3),
							   float scale = 1, bool positiveMode = true)
		{
			LazyLoad();
			if (m_Tower == null) return;

			m_CurrentEffects.Add(fireRateFactor);

			fireRateFactor = fireRateFactorNew;
			
			// Behaviour.
			affector = m_Tower.gameObject.GetComponentInChildren<AttackAffector>();
			if (affector == null) return;
			
			// Save origin fire rate only once.
			if (originalFireRate == -1)
			{
				originalFireRate = affector.fireRate;
			}

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
				newFireRate = originalFireRate * max;
			}
			else
			{
				// Fire rate down
				float min = fireRateFactor;
				foreach (float item in m_CurrentEffects)
				{
					min = Mathf.Min(min, item);
				}
				newFireRate = originalFireRate * min;
			}
			
			// Apply new fire rate
			affector.fireRate = newFireRate;
			oldFireRate = affector.fireRate;

			// Visuals.
			if (m_FireRateFx == null && ratefxPrefab != null)
			{
				m_FireRateFx = Poolable.TryGetPoolable(ratefxPrefab);
				m_FireRateFx.transform.parent = transform;
				m_FireRateFx.transform.localPosition = position;
				m_FireRateFx.transform.localScale *= scale;
			}

			m_Tower.removed += OnRemoved;
		}

		private void Update()
		{
			try
			{
				if (affector == null) affector = m_Tower.gameObject.GetComponentInChildren<AttackAffector>();
				
				if (oldFireRate != affector.fireRate)
				{
					originalFireRate = affector.fireRate;
					
					// Fire rate up
					float max = fireRateFactor;
					foreach (float item in m_CurrentEffects)
					{
						max = Mathf.Max(max, item);
					}
					affector.fireRate = originalFireRate * max;
					oldFireRate = affector.fireRate;
				
				}
			}
			catch (NullReferenceException)
			{
				//
			}
		}

		/// <summary>
		/// Resets the agent's speed 
		/// </summary>
		public void RemoveEffect(float fireRateFactor)
		{
			if (m_Tower == null) return;
			
			m_Tower.removed -= OnRemoved;

			m_CurrentEffects.Remove(fireRateFactor);

			if (m_CurrentEffects.Count != 0)
			{
				return;
			}

			// No more effects
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
			if (m_Tower != null && affector != null)
			{
				affector.fireRate = originalFireRate;
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