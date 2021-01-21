using Zenject;
using HarmonyLib;

namespace SiraUtil.Objects
{
    [HarmonyPatch(typeof(MultiplayerConnectedPlayerInstaller), nameof(MultiplayerConnectedPlayerInstaller.InstallBindings))]
    internal class MultiplayerConnectedPlayerRedecorator
    {
        private static MultiplayerConnectedPlayerGameNoteController _staticMultiplayerGameNotePrefab;
        private static MultiplayerConnectedPlayerBombNoteController _staticMultiplayerBombNotePrefab;

        internal static void Prefix(ref MultiplayerConnectedPlayerInstaller __instance,
            ref MultiplayerConnectedPlayerGameNoteController ____multiplayerGameNoteControllerPrefab,
            ref MultiplayerConnectedPlayerBombNoteController ____multiplayerBombNoteControllerPrefab)
        {
            var mib = __instance as MonoInstallerBase;
            DiContainer Container = Accessors.GetDiContainer(ref mib);

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