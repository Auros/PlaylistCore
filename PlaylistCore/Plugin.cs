using IPA;
using PlaylistCore.UI;
using BS_Utils.Utilities;
using IPALogger = IPA.Logging.Logger;
using HarmonyLib;
using System.Reflection;
using BeatSaberPlaylistsLib;
using UnityEngine;

namespace PlaylistCore
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
            Instance = this;
            Log = logger;

            _harmony = new Harmony("dev.auros.playlistcore");
            BeatSaberPlaylistsLib.Types.Playlist.DefaultCoverImage = Utilities.groupIcon;
            BeatSaberPlaylistsLib.Types.Playlist.LoadWait = new WaitForSeconds(2);
        }

        [OnEnable]
        public void OnEnable()
        {
            _harmony.PatchAll(Assembly.GetExecutingAssembly());
            BSEvents.lateMenuSceneLoadedFresh += MenuLoaded;
        }

        [OnDisable]
        public void OnDisable()
        {
            _harmony.UnpatchAll();
            BSEvents.lateMenuSceneLoadedFresh -= MenuLoaded;
        }

        private void MenuLoaded(ScenesTransitionSetupDataSO _)
        {
            
        }
    }
}