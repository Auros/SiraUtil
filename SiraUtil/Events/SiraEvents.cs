using System;
using Zenject;

namespace SiraUtil.Events
{
    public class SiraEvents
    {
        internal static event EventHandler<SceneContextInstalledArgs> ContextInstalling;

        internal static void SendInstallEvent(string name, SceneContext context, DiContainer container, string mode = null, string transition = null)
        {
            var args = new SceneContextInstalledArgs(name, container, new ModeInfo(transition, mode));
            ContextInstalling.Invoke(context, args);
        }

        internal class SceneContextInstalledArgs : EventArgs
        {
            public string Name { get; }
            public DiContainer Container { get; }
			public ModeInfo ModeInfo { get; }

            public SceneContextInstalledArgs(string name, DiContainer container, ModeInfo modeInfo)
            {
                Name = name;
				ModeInfo = modeInfo;
                Container = container;
            }
        }

		internal class ModeInfo
		{
			public string Gamemode { get; }
			public string Transition { get; }

			public ModeInfo(string gamemode, string transition)
			{
				Gamemode = gamemode ?? "";
				Transition = transition ?? "";
			}
		}
    }
}