using System;
using Zenject;
using HarmonyLib;
using ModestTree;
using System.Linq;
using SiraUtil.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace SiraUtil.Objects
{
    [HarmonyPatch(typeof(BeatmapObjectsInstaller), nameof(BeatmapObjectsInstaller.InstallBindings))]
    internal class BeatmapObjectRedecorator
    {
        private static GameNoteController _staticGameNotePrefab;
        private static BombNoteController _staticBombNotePrefab;

        internal static void Prefix(ref BeatmapObjectsInstaller __instance, ref GameNoteController ____normalBasicNotePrefab, ref BombNoteController ____bombNotePrefab)
        {
            var mib = __instance as MonoInstallerBase;
            DiContainer Container = Accessors.GetDiContainer(ref mib);

            _staticGameNotePrefab = ____normalBasicNotePrefab;
            _staticBombNotePrefab = ____bombNotePrefab;

            if (_staticGameNotePrefab != null)
            {
                ____normalBasicNotePrefab = _staticGameNotePrefab;
            }
            if (_staticBombNotePrefab != null)
            {
                ____bombNotePrefab = _staticBombNotePrefab;
            }

            var normal = InstallModelProviderSystem(Container, ____normalBasicNotePrefab);
            var bomb = InstallModelProviderSystem(Container, ____bombNotePrefab);
            if (normal != null)
            {
                ____normalBasicNotePrefab = normal;
            }
            if (bomb != null)
            {
                ____bombNotePrefab = bomb;
            }
        }

        internal static UXA InstallModelProviderSystem<UXA>(DiContainer Container, UXA original) where UXA : MonoBehaviour
        {
            var providers = Container.Resolve<List<IModelProvider>>().Where(x => x.Type.DerivesFrom(typeof(IPrefabProvider<UXA>)) && x.Priority >= 0);
            var newbaseProvider = providers.OrderByDescending(x => x.Priority).FirstOrDefault();
            if (newbaseProvider != null)
            {
                UXA newPrefab = UnityEngine.Object.Instantiate(original.gameObject).GetComponent<UXA>();
                foreach (var behaviour in newPrefab.gameObject.GetComponentsInChildren<MonoBehaviour>())
                {
                    Container.QueueForInject(behaviour);
                }
                newPrefab.gameObject.name = original.GetType().Name;
                var xPrefabProvider = (IPrefabProvider<UXA>)Activator.CreateInstance(newbaseProvider.Type);
                Container.Inject(xPrefabProvider);
                newPrefab = xPrefabProvider.Modify(newPrefab);
                if (xPrefabProvider.Chain)
                {
                    providers = providers.OrderByDescending(x => x.Priority).Where(x => x.Type != newbaseProvider.Type);
                    foreach (var provider in providers)
                    {
                        xPrefabProvider = (IPrefabProvider<UXA>)Activator.CreateInstance(provider.Type);
                        Container.Inject(xPrefabProvider);
                        if (xPrefabProvider.Chain)
                        {
                            newPrefab = xPrefabProvider.Modify(newPrefab);
                        }
                    }
                }
                return newPrefab;
            }
            return null;
        }
    }
}