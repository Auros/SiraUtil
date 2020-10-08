using System;
using Zenject;

namespace SiraUtil.Events
{
	public class SiraEvents
	{
		internal static event EventHandler<SceneContextInstalledArgs> ContextInstalling;

		internal static void SendInstallEvent(string name, SceneContext context, DiContainer container)
		{
			var args = new SceneContextInstalledArgs(name, container);
			ContextInstalling.Invoke(context, args);
		}

		internal class SceneContextInstalledArgs : EventArgs
		{
			public string Name { get; }
			public DiContainer Container { get; }

			public SceneContextInstalledArgs(string name, DiContainer container)
			{
				Name = name;
				Container = container;
			}
		}
	}
}