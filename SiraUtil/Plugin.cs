using HarmonyLib;
using IPA;
using SiraUtil.Zenject;
using IPALogger = IPA.Logging.Logger;

namespace SiraUtil
{
    [Plugin(RuntimeOptions.DynamicInit)]
    internal class Plugin
    {
        private readonly Harmony _harmony;
        private readonly ZenjectManager _zenjectManager;
        public static IPALogger Log { get; private set; } = null!;
        public const string ID = "dev.auros.sirautil";

        [Init]
        public Plugin(IPALogger logger)
        {
            Log = logger;
            _harmony = new Harmony(ID);
            _zenjectManager = new ZenjectManager();
        }

        [OnEnable]
        public void OnEnable()
        {
            _harmony.PatchAll();
            _zenjectManager.Enable();
        }

        [OnDisable]
        public void OnDisable()
        {
            _zenjectManager.Disable();
            _harmony.UnpatchAll();
        }
    }
}