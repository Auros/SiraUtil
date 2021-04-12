using IPA.Logging;
using ModestTree;
using Zenject;

namespace SiraUtil.Suite.Installers
{
    internal class ParameterCustomInstaller : Installer
    {
        private readonly Logger _logger;

        public ParameterCustomInstaller(Logger logger)
        {
            _logger = logger;
            Assert.IsNotNull(logger);
        }

        public override void InstallBindings()
        {

        }
    }
}