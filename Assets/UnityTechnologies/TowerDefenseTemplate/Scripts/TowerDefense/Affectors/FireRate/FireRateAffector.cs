using ActionGameFramework.Health;
using TowerDefense.Agents;
using UnityEngine;

namespace TowerDefense.Affectors
{
    public class FireRateAffector : PassiveAffector
    {
		/// <summary>
		/// A normalized value to slow agents by
		/// </summary>
		[Range(0, 2)]
		public float fireRateFactor;

		/// <summary>
		/// The slow factor for displaying to the UI
		/// </summary>
		public string fastFactorFormat = "<b>Fire Rate Factor:</b> {0}";

		/// <summary>
		/// The particle system that plays when an entity enters the sphere
		/// </summary>
		public ParticleSystem enterParticleSystem;

		public GameObject fireRateFxPrefab;

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

			DeactivatePowerUP();
		}

		private void DeactivatePowerUP()
		{
			foreach (Targetable enemy in towerTargetter.GetAllTargets())
			{
				try
				{
					RemoveFireRateComponent(enemy.gameObject);
				}
				catch (MissingReferenceException)
				{
					//
				}
			}
		}

		/// <summary>
		/// Attaches a <see cref="AgentFireRateUp" /> to the agent
		/// </summary>
		/// <param name="target">The agent to attach the faster to</param>
		protected void AttachFireRateComponent(GameObject target)
		{
			// Add Agent
			var agent = target.GetComponent<AgentFireRateUp>();
			if (agent == null)
			{
				agent = target.gameObject.AddComponent<AgentFireRateUp>();
			}

			if (fireRateFactor >= 1)
			{
				agent.Initialize(fireRateFactor, fireRateFxPrefab);
			}
			else
            {
				agent.Initialize(fireRateFactor, fireRateFxPrefab, default(Vector3), 1, false);
            }


			// SFX
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
		/// Removes the <see cref="AgentFireRateUp" /> from the agent once it leaves the area
		/// </summary>
		/// <param name="target">The agent to remove the faster from</param>
		protected void RemoveFireRateComponent(GameObject target)
		{
			if (target == null)
			{
				return;
			}

			var agent = target.GetComponent<AgentFireRateUp>();
			if (agent != null)
			{
				agent.RemoveEffect(fireRateFactor);
			}
		}

		/// <summary>
		/// Fired when the targetter aquires a new targetable
		/// </summary>
		protected void OnTargetEntersRange(Targetable other)
		{
			AttachFireRateComponent(other.gameObject);
		}

		/// <summary>
		/// Fired when the targetter aquires loses a targetable
		/// </summary>
		protected void OnTargetExitsRange(Targetable other)
		{
			RemoveFireRateComponent(other.gameObject);
		}
	}
}


