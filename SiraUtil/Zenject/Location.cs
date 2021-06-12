using System;

namespace SiraUtil.Zenject
{
    /// <summary>
    /// Beat Saber specific locations which point to a place to install Zenject bindings.
    /// </summary>
    [Flags]
    public enum Location
    {
        /// <summary>
        /// Represents no binding.
        /// </summary>
        None = 0,

        /// <summary>
        /// Installs your bindings in the app context. Anything installed here will be available in every container. The current backing installer is <see cref="PCAppInit" />
        /// </summary>
        App = 1,

        /// <summary>
        /// Installs your bindings onto the menu. The current backing installer is <see cref="MainSettingsMenuViewControllersInstaller" />
        /// </summary>
        Menu = 2,

        /// <summary>
        /// Installs your bindings onto the standard gameplay (Solo or Party) player. The current backing installer is <see cref="StandardGameplayInstaller" />
        /// </summary>
        StandardPlayer = 4,

        /// <summary>
        /// Installs your bindings onto the campaign player. The current backing installer is <see cref="MissionGameplayInstaller" />
        /// </summary>
        CampaignPlayer = 8,

        /// <summary>
        /// Installs your bindings onto the local multiplayer player. The current backing installer is <see cref="MultiplayerLocalPlayerInstaller" />
        /// </summary>
        MultiPlayer = 16,

        /// <summary>
        /// Installs your bindings onto any player location (Standard, Campaign, or Multiplayer).
        /// </summary>
        Player = StandardPlayer | CampaignPlayer | MultiPlayer,

        /// <summary>
        /// Installs your bindings onto the tutorial. The current backing installer is <see cref="TutorialInstaller" />
        /// </summary>
        Tutorial = 32,

        /// <summary>
        /// Installs your bindings onto GameCore. The current backing installer is <see cref="GameCoreSceneSetup" />
        /// </summary>
        /// <remarks>
        /// Anything specific to the game level will be installed here. It does not necessarily guarantee that anything player specific (audio managers, saber stuff, note spawning stuff)
        /// will be included. Some things you would expect to be in here would be the currently played map (difficulty beatmap).
        /// </remarks>
        GameCore = 64,

        /// <summary>
        /// Installs your bindings onto the Multiplayer Core. The current backing installer is <see cref="MultiplayerCoreInstaller" />
        /// </summary>
        MultiplayerCore = 128,

        /// <summary>
        /// Installs your bindings onto all Players related to singleplayer (Standard, Campaign, or Tutorial).
        /// </summary>
        Singleplayer = StandardPlayer | CampaignPlayer | Tutorial,

        /// <summary>
        /// Installs your bindings onto every connected player in multiplayer. The current backing installer is <see cref="MultiplayerConnectedPlayerInstaller" />
        /// </summary>
        ConnectedPlayer = 256
    }
}