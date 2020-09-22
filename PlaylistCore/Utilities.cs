using HMUI;
using System.IO;
using UnityEngine;
using IPA.Utilities;
using System.Reflection;

namespace PlaylistCore
{
    internal static class Utilities
    {
        internal static FieldAccessor<TextSegmentedControlCellNew, Color>.Accessor NormalCellBGColor = FieldAccessor<TextSegmentedControlCellNew, Color>.GetAccessor("_normalBGColor");
        internal static FieldAccessor<LevelFilteringNavigationController, TabBarViewController>.Accessor TabBar = FieldAccessor<LevelFilteringNavigationController, TabBarViewController>.GetAccessor("_tabBarViewController");

        internal static Sprite groupIcon = LoadSpriteRaw(GetResource(Assembly.GetExecutingAssembly(), "PlaylistCore.Resources.GroupIcon.png"));
        internal static Sprite backButton = LoadSpriteRaw(GetResource(Assembly.GetExecutingAssembly(), "PlaylistCore.Resources.BackArrow.png"));

        public static byte[] GetResource(Assembly asm, string ResourceName)
        {
            Stream stream = asm.GetManifestResourceStream(ResourceName);
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, (int)stream.Length);
            return data;
        }

        public static Texture2D LoadTextureRaw(byte[] file)
        {
            if (file.Length > 0)
            {
                Texture2D Tex2D = new Texture2D(2, 2);
                if (Tex2D.LoadImage(file))
                    return Tex2D;
            }
            return null;
        }

        public static Sprite LoadSpriteFromTexture(Texture2D SpriteTexture, float PixelsPerUnit = 100.0f)
        {
            if (SpriteTexture)
                return Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), PixelsPerUnit);
            return null;
        }
        public static Sprite LoadSpriteRaw(byte[] image, float PixelsPerUnit = 100.0f) => LoadSpriteFromTexture(LoadTextureRaw(image), PixelsPerUnit);
    }
}