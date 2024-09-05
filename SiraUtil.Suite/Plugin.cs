using IPA;
using SiraUtil.Extras;
using SiraUtil.Objects.Beatmap;
using SiraUtil.Sabers;
using SiraUtil.Suite.Installers;
using SiraUtil.Suite.Tests.Installers;
using SiraUtil.Suite.Tests.Sabers;
using SiraUtil.Suite.Tests.Web;
using SiraUtil.Zenject;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;

namespace SiraUtil.Suite
{
    [Plugin(RuntimeOptions.DynamicInit), NoEnableDisable]
    public class Plugin
    {
        internal static IPALogger Log { get; set; } = null!;

        [Init]
        public Plugin(IPALogger logger, Zenjector zenjector)
        {
            Log = logger;
            zenjector.UseAutoBinder();
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
            zenjector.UseHttpService();

            zenjector.UseSiraSync(Web.SiraSync.SiraSyncServiceType.GitHub, "Auros", "SiraUtil");
            zenjector.Install(Location.Menu, Container => Container.BindInterfacesTo<WebTest>().AsSingle());

            zenjector.Install(Location.Player | Location.Tutorial, Container =>
            {
                Container.BindInterfacesTo<SpawnFullSaberTest>().AsSingle();
                Container.BindInstance(SaberModelRegistration.Create<TestSaberModelController>(100)).AsSingle();
                
                Container.RegisterRedecorator(new BasicNoteRegistration(Create2, 20));
            });
            zenjector.Install(Location.ConnectedPlayer, Container =>
            {
                Container.RegisterRedecorator(new ConnectedPlayerNoteRegistration(Create));
            });
        }

        private GameNoteController Create2(GameNoteController before)
        {
            Log.Info("ASEklsgkjpesrg");

            GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
            g.transform.SetParent(before.transform.GetChild(0));
            g.transform.localPosition = Vector3.zero;
            g.transform.localScale = new Vector3(0.05f, 0.05f, 0.6f);

            return before;
        }

        private MultiplayerConnectedPlayerGameNoteController Create(MultiplayerConnectedPlayerGameNoteController before)
        {
            Log.Info("hello");

            GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            g.transform.SetParent(before.transform.GetChild(0));
            g.transform.localPosition = new Vector3(0f, 0f, 0);
            g.transform.localScale *= 3f;
            
            return before;
        }
    }
}