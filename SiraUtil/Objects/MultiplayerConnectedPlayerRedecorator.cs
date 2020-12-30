using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Zenject;

namespace SiraUtil.Objects
{
    [HarmonyPatch(typeof(MultiplayerConnectedPlayerInstaller), nameof(MultiplayerConnectedPlayerInstaller.InstallBindings))]
    internal class MultiplayerConnectedPlayerRedecorator
    {
        private static MultiplayerConnectedPlayerGameNoteController _staticMultiplayerGameNotePrefab;
        private static MultiplayerConnectedPlayerBombNoteController _staticMultiplayerBombNotePrefab;

        internal static void Prefix(ref MultiplayerConnectedPlayerInstaller __instance, ref MultiplayerConnectedPlayerGameNoteController ____multiplayerGameNoteControllerPrefab,
            ref MultiplayerConnectedPlayerBombNoteController ____multiplayerBombNoteControllerPrefab, ref IConnectedPlayer ____connectedPlayer)
        {
            var mib = __instance as MonoInstallerBase;
            DiContainer Container = Accessors.GetDiContainer(ref mib);

            Container.BindInstance(____connectedPlayer).WithId("sirautil.connectedplayer");
            Container.QueueForInject(____connectedPlayer);

            _staticMultiplayerGameNotePrefab = ____multiplayerGameNoteControllerPrefab;
            _staticMultiplayerBombNotePrefab = ____multiplayerBombNoteControllerPrefab;

            if (_staticMultiplayerGameNotePrefab != null)
            {
                ____multiplayerGameNoteControllerPrefab = _staticMultiplayerGameNotePrefab;
            }
            if (_staticMultiplayerBombNotePrefab != null)
            {
                ____multiplayerBombNoteControllerPrefab = _staticMultiplayerBombNotePrefab;
            }

            var normal = BeatmapObjectRedecorator.InstallModelProviderSystem(Container, ____multiplayerGameNoteControllerPrefab);
            var bomb = BeatmapObjectRedecorator.InstallModelProviderSystem(Container, ____multiplayerBombNoteControllerPrefab);
            if (normal != null)
            {
                ____multiplayerGameNoteControllerPrefab = normal;
            }
            if (bomb != null)
            {
                ____multiplayerBombNoteControllerPrefab = bomb;
            }
        }
    }
}
