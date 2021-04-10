using IPA;
using SiraUtil.Suite.Installers;
using SiraUtil.Zenject;
using IPALogger = IPA.Logging.Logger;

namespace SiraUtil.Suite
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        internal static IPALogger Log { get; set; } = null!;

        [Init]
        public Plugin(IPALogger logger, Zenjector zenjector)
        {
            Log = logger;
            zenjector.Install<GenericCustomInstaller>(Location.Menu);
            zenjector.Install<MonoCustomInstaller>(Location.Tutorial);
            zenjector.Install<OtherCustomInstaller, GameplayCoreInstaller>();
            zenjector.Install<ParameterCustomInstaller>(Location.App, logger);
        }

        [OnEnable]
        public void OnEnable()
        {

        }

        [OnDisable]
        public void OnDisable()
        {

        }
    }
}