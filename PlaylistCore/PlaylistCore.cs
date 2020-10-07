using System.Linq;
using BS_Utils.Utilities;
using BeatSaberPlaylistsLib;
using PlaylistCore.Overrides;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;

namespace PlaylistCore
{
    public static class PlaylistCore
    {
        internal static IAnnotatedBeatmapLevelCollection[] playlistTabData;

        public static async void SetupCustomPlaylists(object tabBar)
        {

            List<IAnnotatedBeatmapLevelCollection> levelCollections = new List<IAnnotatedBeatmapLevelCollection>();
            levelCollections.AddRange(playlistTabData);

            var data = await GetStuff();
            
            for (int i = 0; i < data.Item3.Count(); i++)
            {
                var customGroup = new CustomPlaylistGroup(data.Item3.ElementAt(i));
                levelCollections.Add(customGroup);
            }
            levelCollections.AddRange(data.Item2);
            tabBar.SetField("annotatedBeatmapLevelCollections", levelCollections.ToArray());
        }

        public static Task<(PlaylistManager, IPlaylist[], IEnumerable<PlaylistManager>)> GetStuff()
        {
            var tcs = new TaskCompletionSource<(PlaylistManager, IPlaylist[], IEnumerable<PlaylistManager>)>();
            Task.Run(() =>
            {
                var manager = PlaylistManager.DefaultManager;
                var playlists = manager.GetAllPlaylists();
                var groups = manager.GetChildManagers();

                tcs.SetResult((manager, playlists, groups));
            });

            return tcs.Task;
        }

        internal static IAnnotatedBeatmapLevelCollection[] SetupGroup(PlaylistManager manager, bool withBackButton)
        {
            var sw = Stopwatch.StartNew();
            List<IAnnotatedBeatmapLevelCollection> levelCollections = new List<IAnnotatedBeatmapLevelCollection>();
            if (withBackButton)
            {
                var backButton = new CustomPlaylistBackButton(manager.Parent);
                levelCollections.Add(backButton);
            }
            var groups = manager.GetChildManagers();
            for (int i = 0; i < groups.Count(); i++)
            {
                var customGroup = new CustomPlaylistGroup(groups.ElementAt(i));
                levelCollections.Add(customGroup);
            }
            levelCollections.AddRange(manager.GetAllPlaylists());
            sw.Stop();
            Plugin.Log.Info($"Took {sw.Elapsed.TotalSeconds} to do the thing.");
            return levelCollections.ToArray();
        }
    }
}