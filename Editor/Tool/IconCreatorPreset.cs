#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Jeomseon.Unity.EditorToolkit.Editor.Tool
{
    [CreateAssetMenu(fileName = "IconCreatorPreset", menuName = "Jeomseon/Icon Creator Preset")]
    public sealed class IconCreatorPreset : ScriptableObject
    {
        [Min(1), FormerlySerializedAs("size")] public int Size = 128;
        [Min(1), FormerlySerializedAs("divideCount")] public int DivideCount = 4;
        [FormerlySerializedAs("textureType")] public TextureImporterType TextureType = TextureImporterType.Sprite;
        [FormerlySerializedAs("spriteImportMode")] public SpriteImportMode SpriteImportMode = SpriteImportMode.Multiple;
        [FormerlySerializedAs("generateTmpSpriteAsset")] public bool GenerateTmpSpriteAsset = true;
        [FormerlySerializedAs("defaultIconSources")] public List<Sprite> DefaultIconSources = new();
    }
}
#endif
