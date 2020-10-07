using System.IO;
using UnityEngine;
using System.Threading;
using BeatSaberPlaylistsLib;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace PlaylistCore.Overrides
{
    public class CustomPlaylistGroup : IPlaylist
    {
        public CustomPlaylistGroup(PlaylistManager manager)
        {
            PlaylistManager = manager;
            try
            {
                IPreviewBeatmapLevel[] levels = manager.GetAllPlaylists(false, out _).SelectMany(p => p.Where(s => s.PreviewBeatmapLevel != null)).ToArray();
                if (levels != null && levels.Length > 0)
                    _beatmapLevelCollection = new DummyBeatmapLevelCollection(levels);
            }
            catch (Exception ex)
            {
                // Just in case.
                Plugin.Log.Debug($"Error creating level collection for playlist group {collectionName}: {ex.Message}");
                Plugin.Log.Debug(ex);
            }
        }

        private string _collectionName;
        public string collectionName
        {
            get
            {
                if (string.IsNullOrEmpty(_collectionName))
                    _collectionName = new DirectoryInfo(PlaylistManager.PlaylistPath).Name;
                return _collectionName;
            }
        }

        public Sprite coverImage => BeatSaberPlaylistsLib.Utilities.DefaultSprite;

        private IBeatmapLevelCollection _beatmapLevelCollection;
        public IBeatmapLevelCollection beatmapLevelCollection
        {
            get
            {
                if (_beatmapLevelCollection == null)
                    _beatmapLevelCollection = new DummyBeatmapLevelCollection();
                return _beatmapLevelCollection;
            }
        }
        public PlaylistManager PlaylistManager { get; }

        internal class DummyBeatmapLevelCollection : IBeatmapLevelCollection
        {
            public DummyBeatmapLevelCollection() { }
            public DummyBeatmapLevelCollection(IPreviewBeatmapLevel[] levels)
            {
                _beatmapLevels = levels;
            }

            private IPreviewBeatmapLevel[] _beatmapLevels;
            public IPreviewBeatmapLevel[] beatmapLevels => _beatmapLevels ?? Array.Empty<IPreviewBeatmapLevel>();
        }

        internal class DummyPreviewBeatmapLevel : IPreviewBeatmapLevel
        {
            public string levelID => "";

            public string songName => "";

            public string songSubName => "";

            public string songAuthorName => "";

            public string levelAuthorName => "";

            public float beatsPerMinute => 0f;

            public float songTimeOffset => 0f;

            public float shuffle => 0f;

            public float shufflePeriod => 0f;

            public float previewStartTime => 0f;

            public float previewDuration => 0f;

            public float songDuration => 0f;

            public EnvironmentInfoSO environmentInfo => null;

            public EnvironmentInfoSO allDirectionsEnvironmentInfo => null;

            public PreviewDifficultyBeatmapSet[] previewDifficultyBeatmapSets => null;

            public Task<Texture2D> GetCoverImageTexture2DAsync(CancellationToken cancellationToken)
            {
                return null;
            }

            public Task<AudioClip> GetPreviewAudioClipAsync(CancellationToken cancellationToken)
            {
                return null;
            }
        }
    }
}