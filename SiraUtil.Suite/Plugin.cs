using IPA;
using SiraUtil.Suite.Installers;
using SiraUtil.Suite.Tests.Installers;
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

            zenjector.Install(Location.Menu, Container => { });
            zenjector.Install<MainSettingsMenuViewControllersInstaller>(Container => { });

            zenjector.Expose<FlickeringNeonSign>("MenuEnvironment");
            zenjector.Mutate<PlatformLeaderboardViewController>("MenuViewControllers", (_, __) => { });

            zenjector.Install<AffinityTestInstaller>(Location.App);
            zenjector.UseLogger(logger);
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