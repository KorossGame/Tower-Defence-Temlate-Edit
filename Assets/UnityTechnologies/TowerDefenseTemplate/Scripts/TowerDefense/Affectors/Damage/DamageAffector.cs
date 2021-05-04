using ActionGameFramework.Health;
using System.Collections;
using System.Collections.Generic;
using TowerDefense.Agents;
using TowerDefense.Towers;
using UnityEngine;

namespace TowerDefense.Affectors
{
    public class DamageAffector : PassiveAffector
    {
		/// <summary>
		/// A normalized value to increase damage by
		/// </summary>
		[Range(0, 2)]
		public float damageFactor;

		/// <summary>
		/// The slow factor for displaying to the UI
		/// </summary>
		public string damageFactorFormat = "<b>Damage Factor:</b> {0}";

		/// <summary>
		/// The particle system that plays when an entity enters the sphere
		/// </summary>
		public ParticleSystem enterParticleSystem;

		public GameObject damageFxPrefab;

		/// <summary>
		/// The audio source that plays when an entity enters the sphere
		/// </summary>
		public AudioSource audioSource;

		/// <summary>
		/// Subsribes to the relevant targetter events
		/// </summary>
		protected void Awake()
		{
			towerTargetter.targetEntersRange += OnTargetEntersRange;
			towerTargetter.targetExitsRange += OnTargetExitsRange;
		}

		/// <summary>
		/// Unsubsribes from the relevant targetter events
		/// </summary>
		protected void OnDestroy()
		{
			towerTargetter.targetEntersRange -= OnTargetEntersRange;
			towerTargetter.targetExitsRange -= OnTargetExitsRange;
		}

        /// <summary>
        /// Attaches a <see cref="AgentDamageUp" /> to the agent
        /// </summary>
        /// <param name="target">The agent to attach the faster to</param>
        protected void AttachDamageComponent(GameObject target)
		{
			var agent = target.GetComponent<AgentDamageUp>();

			if (agent == null)
			{
				agent = target.AddComponent<AgentDamageUp>();
			}

			if (damageFactor >= 1)
			{
				agent.Initialize(damageFactor, damageFxPrefab);
			}
			else
			{
				agent.Initialize(damageFactor, damageFxPrefab, default(Vector3), 1, false);
			}

			if (enterParticleSystem != null)
			{
				enterParticleSystem.Play();
			}
			if (audioSource != null)
			{
				audioSource.Play();
			}
		}

		/// <summary>
		/// Removes the <see cref="AgentDamageUp" /> from the agent once it leaves the area
		/// </summary>
		/// <param name="target">The agent to remove the faster from</param>
		protected void RemoveDamageComponent(Agent target)
		{
			if (target == null)
			{
				return;
			}
			var agent = target.gameObject.GetComponent<AgentDamageUp>();
			if (agent != null)
			{
				agent.RemoveDamage(damageFactor);
			}
		}

		/// <summary>
		/// Fired when the targetter aquires a new targetable
		/// </summary>
		protected void OnTargetEntersRange(Targetable other)
		{
			if (!isActive) return;
			AttachDamageComponent(other.gameObject);
		}

		/// <summary>
		/// Fired when the targetter aquires loses a targetable
		/// </summary>
		protected void OnTargetExitsRange(Targetable other)
		{
			var searchable = other as Agent;
			if (searchable == null)
			{
				return;
			}
			RemoveDamageComponent(searchable);
		}
	}
}
