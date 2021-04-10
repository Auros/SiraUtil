using IPA;
using SiraUtil.Suite.Installers;
using SiraUtil.Zenject;
using IPALogger = IPA.Logging.Logger;

namespace SiraUtil.Suite
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        [Init]
        public Plugin(IPALogger logger, Zenjector zenjector)
        {
            zenjector.Install<GenericCustomInstaller>(Location.Menu);
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