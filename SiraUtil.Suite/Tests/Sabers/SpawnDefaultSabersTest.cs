using SiraUtil.Logging;
using SiraUtil.Sabers;
using UnityEngine;
using Zenject;

namespace SiraUtil.Suite.Tests.Sabers
{
    internal class SpawnDefaultSabersTest : IInitializable
    {
        private readonly SiraLog _siraLog;
        private readonly SaberModelProvider _saberModelProvider;

        public SpawnDefaultSabersTest(SiraLog siraLog, SaberModelProvider saberModelProvider)
        {
            _siraLog = siraLog;
            _saberModelProvider = saberModelProvider;
        }

        public void Initialize()
        {
            _siraLog.Info("Spawning new sabers...");
            SaberModelController modelA = _saberModelProvider.NewModel(SaberType.SaberA);
            SaberModelController modelB = _saberModelProvider.NewModel(SaberType.SaberB);
            SaberModelController modelC = _saberModelProvider.NewModel(null);
            _siraLog.Info("Spawned new sabers.");

            modelA.name = "NEW SABER MODEL A";
            modelA.GetComponentInChildren<SaberTrail>().enabled = false;

            modelB.name = "NEW SABER MODEL B";
            modelB.GetComponentInChildren<SaberTrail>().enabled = false;

            modelC.name = "NEW SABER MODEL C";
            modelC.GetComponentInChildren<SaberTrail>().enabled = false;

            modelA.transform.position = new Vector3(-0.5f, 1f, 0f);
            modelB.transform.position = new Vector3(0.5f, 1f, 0f);
            modelC.transform.position = new Vector3(0f, 1f, 0f);
        }
    }
}