using IPA;
using HarmonyLib;
using System.Reflection;
using IPALogger = IPA.Logging.Logger;

namespace SiraUtil
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; set; }

        private readonly Harmony _harmony;

        [Init]
        public Plugin(IPALogger logger)
        {
            Log = logger;
            Instance = this;

            _harmony = new Harmony("dev.auros.sirautil");
        }

        [OnEnable]
        public void OnEnable()
        {
            _harmony?.PatchAll(Assembly.GetExecutingAssembly());

            //ExtraSabers.Touch();
        }

        [OnDisable]
        public void OnDisable()
        {
            //ExtraSabers.Untouch();

            _harmony?.UnpatchAll();
        }
    }
}