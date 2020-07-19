using IPA;
using HarmonyLib;
using System.Reflection;
using IPALogger = IPA.Logging.Logger;
using SiraUtil.Sabers;
using UnityEngine;

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

            var go = new GameObject();
            Object.DontDestroyOnLoad(go);
            var ye = go.AddComponent<BruhModelController>();

            var smp = new SaberModelProvider(100, ye);
            //SaberModelProvider.AddProvider(smp);
            //ExtraSabers.Touch();
        }

        [OnDisable]
        public void OnDisable()
        {
            //ExtraSabers.Untouch();

            _harmony?.UnpatchAll();
        }
    }

    public class BruhModelController : MonoBehaviourSaberModelController
    {
        public override void Init(Transform parent, SaberType saberTypeObject)
        {
            transform.SetParent(parent, false);
            Plugin.Log.Info("Yeah, i initted");
        }
    }
}