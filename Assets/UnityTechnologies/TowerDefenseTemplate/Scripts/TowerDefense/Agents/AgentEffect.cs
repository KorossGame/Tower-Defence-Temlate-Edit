using TowerDefense.Towers;
using UnityEngine;

namespace TowerDefense.Agents
{
	/// <summary>
	/// A component that will apply various effects on an agent
	/// </summary>
	public abstract class AgentEffect : MonoBehaviour
	{
		/// <summary>
		/// References to the agent that will be affected
		/// </summary>
		protected Agent m_Agent;
		public Tower m_Tower;

		public virtual void Awake()
		{
			LazyLoad();
		}

		/// <summary>
		/// A lazy way to ensure that <see cref="m_Agent"/> will not be null
		/// </summary>
		public virtual void LazyLoad()
		{
			if (m_Agent == null)
			{
				m_Agent = GetComponent<Agent>();
			}
			if (m_Tower == null)
            {
				m_Tower = GetComponent<Tower>();
			}
		}
	}
}