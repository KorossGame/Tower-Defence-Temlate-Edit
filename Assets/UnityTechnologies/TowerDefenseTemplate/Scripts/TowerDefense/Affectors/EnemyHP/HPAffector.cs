using ActionGameFramework.Health;
using System.Collections;
using System.Collections.Generic;
using TowerDefense.Agents;
using UnityEngine;

namespace TowerDefense.Affectors
{
    public class HPAffector : PassiveAffector
    {
		/// <summary>
		/// A normalized value to slow agents by
		/// </summary>
		[Range(0, 2)]
		public float hpFactor;

		/// <summary>
		/// The slow factor for displaying to the UI
		/// </summary>
		public string hpFactorFormat = "<b>HP Factor:</b> {0}";

		/// <summary>
		/// The particle system that plays when an entity enters the sphere
		/// </summary>
		public ParticleSystem enterParticleSystem;

		public GameObject hpFxPrefab;

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
		/// Attaches a <see cref="AgentHPUp" /> to the agent
		/// </summary>
		/// <param name="target">The agent to attach the faster to</param>
		protected void AttachHPComponent(Agent target)
		{
			var agent = target.GetComponent<AgentHPUp>();
			if (agent == null)
			{
				agent = target.gameObject.AddComponent<AgentHPUp>();
			}

			if (hpFactor <= 1)
			{
				agent.Initialize(hpFactor, hpFxPrefab, target.appliedEffectOffset, target.appliedEffectScale);
			}
			else
			{
				agent.Initialize(hpFactor, hpFxPrefab, target.appliedEffectOffset, target.appliedEffectScale, false);
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
		/// Removes the <see cref="AgentHPUp" /> from the agent once it leaves the area
		/// </summary>
		/// <param name="target">The agent to remove the faster from</param>
		protected void RemoveHPComponent(Agent target)
		{
			if (target == null)
			{
				return;
			}
			var fastComponent = target.gameObject.GetComponent<AgentHPUp>();
			if (fastComponent != null)
			{
				fastComponent.RemoveSlow(hpFactor);
			}
		}

		/// <summary>
		/// Fired when the targetter aquires a new targetable
		/// </summary>
		protected void OnTargetEntersRange(Targetable other)
		{
			if (!isActive) return;

			var agent = other as Agent;
			if (agent == null)
			{
				return;
			}
			AttachHPComponent(agent);
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
			RemoveHPComponent(searchable);
		}
	}
}
