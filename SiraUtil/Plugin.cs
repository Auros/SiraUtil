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

        public Harmony Harmony { get; }

        [Init]
        public Plugin(IPALogger logger)
        {
            Log = logger;
            Instance = this;
            Harmony = new Harmony("dev.auros.sirautil");
        }

        [OnEnable]
        public void OnEnable()
        {
            Harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        [OnDisable]
        public void OnDisable()
        {
            Harmony.UnpatchAll();
        }
    }
}