using Zenject;
using System.Collections.Generic;

namespace SiraUtil.Zenject
{
    public class Zenjector
    {
        public string Name { get; }
        internal bool Enabled { get; private set; } = true;
        internal bool AutoControl { get; private set; } = true;
        internal IList<InstallBuilder> Builders { get; } = new List<InstallBuilder>();

        internal Zenjector(string name)
        {
            Name = name;
        }

        public void Enable()
        {
            Enabled = true;
        }

        public void Disable()
        {
            Enabled = false;
        }

        public void Auto()
        {
            AutoControl = true;
        }

        public void Manual()
        {
            AutoControl = false;
        }

        public InstallBuilder OnApp(string destination = null)
        {
            return OnGeneric(destination);
        }

        public InstallBuilder OnMenu(string destination = null)
        {
            return OnGeneric(destination);
        }

        public InstallBuilder OnGame(string destination = null)
        {
            return OnGeneric(destination);
        }

        public InstallBuilder OnApp<T>() where T : IInstaller
        {
            var ib = new InstallBuilder(typeof(T));
            Builders.Add(ib);
            ib.On(nameof(PCAppInit));
            return ib;
        }

        public InstallBuilder OnMenu<T>() where T : IInstaller
        {
            var ib = new InstallBuilder(typeof(T));
            Builders.Add(ib);
            ib.On("Menu");
            return ib;
        }

        public InstallBuilder OnGame<T>() where T : IInstaller
        {
            var ib = new InstallBuilder(typeof(T));
            Builders.Add(ib);
            ib.On(nameof(GameCoreSceneSetup));
            return ib;
        }

        public InstallBuilder Register<T>() where T : IInstaller
        {
            var ib = new InstallBuilder();
            Builders.Add(ib);
            return ib.Register<T>();
        }

        public InstallBuilder On<T>()
        {
            return OnGeneric(nameof(T));
        }

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