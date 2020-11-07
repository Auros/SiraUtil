using Zenject;
using System.Collections.Generic;

namespace SiraUtil.Zenject
{
    /// <summary>
    /// An extensible system for creating Zenject hooks in a mod with a focus on customizability.
    /// </summary>
    public class Zenjector
    {
        /// <summary>
        /// The name of the Zenjector
        /// </summary>
        public string Name { get; }
        internal bool Enabled { get; private set; } = true;
        internal bool AutoControl { get; private set; } = true;
        internal IList<InstallBuilder> Builders { get; } = new List<InstallBuilder>();

        internal Zenjector(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Enable the Zenjector. (Enabled by default)
        /// </summary>
        public void Enable()
        {
            Enabled = true;
        }

        /// <summary>
        /// Disable this Zenjector.
        /// </summary>
        public void Disable()
        {
            Enabled = false;
        }

        /// <summary>
        /// Allow SiraUtil to automatically handle the state of your installers based on the status of your plugin. (Enabled by default)
        /// </summary>
        public void Auto()
        {
            AutoControl = true;
        }

        /// <summary>
        /// Disallow SiraUtil from automatically handling the state of your installers.
        /// </summary>
        public void Manual()
        {
            AutoControl = false;
        }

        /// <summary>
        /// Install your installer on the app installer (project context), any bindings made here are available throughout the entire game.
        /// </summary>
        /// <typeparam name="T">The type of your installer.</typeparam>
        public InstallBuilder OnApp<T>() where T : IInstaller
        {
            var ib = new InstallBuilder(typeof(T));
            Builders.Add(ib);
            ib.On(typeof(PCAppInit).FullName);
            return ib;
        }

        /// <summary>
        /// Installs your installer on the menu scene.
        /// </summary>
        /// <typeparam name="T">The type of your installer.</typeparam>
        public InstallBuilder OnMenu<T>() where T : IInstaller
        {
            var ib = new InstallBuilder(typeof(T));
            Builders.Add(ib);
            ib.On("Menu");
            return ib;
        }

        /// <summary>
        /// Installs your installer on the game scene.
        /// </summary>
        /// <typeparam name="T">The type of your installer.</typeparam>
        public InstallBuilder OnGame<T>() where T : IInstaller
        {
            var ib = new InstallBuilder(typeof(T));
            Builders.Add(ib);
            ib.On(typeof(GameCoreSceneSetup).FullName);
            return ib;
        }

        /// <summary>
        /// Installs your installer as the game scene is setting up.
        /// </summary>
        /// <typeparam name="T">The type of your installer.</typeparam>
        /// <param name="onGameSetup">Whether or not the installer is installed along during the game core setup. If your objects depend on beatmap elements like the <see cref="AudioTimeSyncController"/> or <see cref="BeatmapObjectManager"/>, set this to false as they don't exist in setup in multiplayer.</param>
        public InstallBuilder OnGame<T>(bool onGameSetup = true) where T : IInstaller
        {
            var ib = new InstallBuilder(typeof(T));
            Builders.Add(ib);
            ib.On(onGameSetup ? typeof(GameCoreSceneSetup).FullName : typeof(GameplayCoreInstaller).FullName);
            return ib;
        }

        /// <summary>
        /// Installs an installer.
        /// </summary>
        /// <typeparam name="T">The type of your installer.</typeparam>
        public InstallBuilder Register<T>() where T : IInstaller
        {
            var ib = new InstallBuilder();
            Builders.Add(ib);
            return ib.Register<T>();
        }

        /// <summary>
        /// Provides a destination for your installer to be installed on.
        /// </summary>
        /// <typeparam name="T">The type of the destination.</typeparam>
        public InstallBuilder On<T>()
        {
            return OnGeneric(typeof(T).FullName);
        }

        /// <summary>
        /// Provides a destination for your installer to be installed on.
        /// </summary>
        /// <param name="destination">The name of the destination.</param>
        public InstallBuilder On(string destination)
        {
            return OnGeneric(destination);
        }

        private InstallBuilder OnGeneric(string destination = null)
        {
            var ib = new InstallBuilder();
            Builders.Add(ib);
            return ib.On(destination);
        }
    }
}