#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Jeomseon.Editor.Tool
{
    [CreateAssetMenu(fileName = "IconCreatorPreset", menuName = "Jeomseon/Icon Creator Preset")]
    public sealed class IconCreatorPreset : ScriptableObject
    {
        [Min(1)] public int size = 128;
        [Min(1)] public int divideCount = 4;
        public TextureImporterType textureType = TextureImporterType.Sprite;
        public SpriteImportMode spriteImportMode = SpriteImportMode.Multiple;
        public bool generateTmpSpriteAsset = true;
        public List<Sprite> defaultIconSources = new();
    }
}
#endif
