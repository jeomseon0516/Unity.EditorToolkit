#if UNITY_EDITOR
using UnityEngine;

namespace Jeomseon.Editor.GUI
{
    public static class GUIStyleTexture
    {
        public static Texture2D Create(Color32 color, GUIStyle guiStyle)
        {
            Color32[] pixels = new Color32[guiStyle.border.horizontal * guiStyle.border.vertical];

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }

            Texture2D texture = new(guiStyle.border.horizontal, guiStyle.border.vertical);
            texture.SetPixels32(pixels);
            texture.Apply();

            return texture;
        }
    }
}
#endif
