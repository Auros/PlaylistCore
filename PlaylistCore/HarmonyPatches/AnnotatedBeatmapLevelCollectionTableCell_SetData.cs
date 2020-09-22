using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using BeatSaberPlaylistsLib.Types;
using BS_Utils.Utilities;
using HarmonyLib;
using IPA.Utilities;
using UnityEngine.UI;

/// <summary>
/// See https://github.com/pardeike/Harmony/wiki for a full reference on Harmony.
/// </summary>
namespace PlaylistCore.HarmonyPatches
{
    /// <summary>
    /// This patches ClassToPatch.MethodToPatch(Parameter1Type arg1, Parameter2Type arg2)
    /// </summary>
    [HarmonyPatch(typeof(AnnotatedBeatmapLevelCollectionTableCell), nameof(AnnotatedBeatmapLevelCollectionTableCell.SetData),
        new Type[] { // List the Types of the method's parameters.
        typeof(IAnnotatedBeatmapLevelCollection)})]
    public class AnnotatedBeatmapLevelCollectionTableCell_SetData
    {
        public static readonly ConditionalWeakTable<IDeferredSpriteLoad, AnnotatedBeatmapLevelCollectionTableCell> EventTable = new ConditionalWeakTable<IDeferredSpriteLoad, AnnotatedBeatmapLevelCollectionTableCell>();
        public static readonly FieldAccessor<AnnotatedBeatmapLevelCollectionTableCell, Image>.Accessor CoverImageAccessor
            = FieldAccessor<AnnotatedBeatmapLevelCollectionTableCell, Image>.GetAccessor("_coverImage");
        public static readonly FieldAccessor<AnnotatedBeatmapLevelCollectionTableCell, IAnnotatedBeatmapLevelCollection>.Accessor BeatmapCollectionAccessor
             = FieldAccessor<AnnotatedBeatmapLevelCollectionTableCell, IAnnotatedBeatmapLevelCollection>.GetAccessor("_annotatedBeatmapLevelCollection");
        /// <summary>
        /// This code is run before the original code in MethodToPatch is run.
        /// </summary>
        /// <param name="__instance">The instance of ClassToPatch</param>
        /// <param name="arg1">The Parameter1Type arg1 that was passed to MethodToPatch</param>
        /// <param name="____privateFieldInClassToPatch">Reference to the private field in ClassToPatch named '_privateFieldInClassToPatch', 
        ///     added three _ to the beginning to reference it in the patch. Adding ref means we can change it.</param>
        static void Postfix(AnnotatedBeatmapLevelCollectionTableCell __instance, ref IAnnotatedBeatmapLevelCollection annotatedBeatmapLevelCollection, ref Image ____coverImage)
        {
            AnnotatedBeatmapLevelCollectionTableCell cell = __instance;
            if (annotatedBeatmapLevelCollection is IDeferredSpriteLoad deferredSpriteLoad)
            {
                if (deferredSpriteLoad.SpriteWasLoaded)
                {
                    Plugin.Log.Info($"Sprite was already loaded for {(deferredSpriteLoad as IAnnotatedBeatmapLevelCollection).collectionName}");
                }
                if (EventTable.TryGetValue(deferredSpriteLoad, out AnnotatedBeatmapLevelCollectionTableCell existing))
                {
                    EventTable.Remove(deferredSpriteLoad);
                }
                EventTable.Add(deferredSpriteLoad, cell);
                deferredSpriteLoad.SpriteLoaded -= OnSpriteLoaded;
                deferredSpriteLoad.SpriteLoaded += OnSpriteLoaded;
            }
        }


        public static void OnSpriteLoaded(object sender, EventArgs e)
        {
            if (sender is IDeferredSpriteLoad deferredSpriteLoad)
            {
                if (EventTable.TryGetValue(deferredSpriteLoad, out AnnotatedBeatmapLevelCollectionTableCell tableCell))
                {
                    IAnnotatedBeatmapLevelCollection collection = BeatmapCollectionAccessor(ref tableCell);
                    if (collection == deferredSpriteLoad)
                    {
                        Plugin.Log.Info($"Updating image for {collection.collectionName}");
                        CoverImageAccessor(ref tableCell).sprite = deferredSpriteLoad.Sprite;
                    }
                    else
                    {
                        Plugin.Log.Warn($"Collection '{collection.collectionName}' is not {(deferredSpriteLoad as IAnnotatedBeatmapLevelCollection).collectionName}");
                        EventTable.Remove(deferredSpriteLoad);
                        deferredSpriteLoad.SpriteLoaded -= OnSpriteLoaded;
                    }
                }
                else
                {
                    Plugin.Log.Warn($"{(deferredSpriteLoad as IAnnotatedBeatmapLevelCollection).collectionName} is not in the EventTable.");
                    deferredSpriteLoad.SpriteLoaded -= OnSpriteLoaded;
                }
            }
            else
            {
                Plugin.Log.Warn($"Wrong sender type for deferred sprite load: {sender?.GetType().Name ?? "<NULL>"}");
            }
        }
    }
}