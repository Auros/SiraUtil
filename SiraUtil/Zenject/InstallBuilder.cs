using System;
using Zenject;
using ModestTree;

namespace SiraUtil.Zenject
{
    public class InstallBuilder
    {
        internal Type Type { get; private set; }
        internal string Destination { get; private set; }
        internal object[] Parameters { get; private set; }

        internal InstallBuilder() { }
        internal InstallBuilder(Type type)
        {
            Type = type;
        }

        public InstallBuilder WithParameters(params object[] parameters)
        {
            Parameters = parameters;
            return this;
        }

        public InstallBuilder On(string destination)
        {
            Destination = destination;
            return this;
        }

        public InstallBuilder On<T>()
        {
            On(nameof(T));
            return this;
        }

        public InstallBuilder Register<T>() where T : IInstaller
        {
            Type = typeof(T);
            return this;
        }

        internal void Validate()
        {
            Assert.IsNotNull(Type, $"Zenject Registration must have a type. {Utilities.ASSERTHIT}");
            Assert.That(Type.DerivesFrom<IInstaller>(), $"Type must be an IInstaller {Utilities.ASSERTHIT}");
            if (string.IsNullOrEmpty(Destination))
            {
                throw new ArgumentNullException($"{nameof(Type)}:{nameof(Destination)}", "Installer registration needs a destination.");
            }
        }
    }
}