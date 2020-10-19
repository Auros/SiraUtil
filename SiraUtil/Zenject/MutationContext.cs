using Zenject;
using UnityEngine;
using System.Collections.Generic;

namespace SiraUtil.Zenject
{
	public class MutationContext
	{
		private readonly List<MonoBehaviour> _injectables;

		public DiContainer Container { get; }

		internal MutationContext(DiContainer container, List<MonoBehaviour> injectables)
		{
			Container = container;
			_injectables = injectables;
		}

		public void AddInjectable(MonoBehaviour behaviour)
		{
			_injectables.Add(behaviour);
		}
	}
}