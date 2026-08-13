using UnityEngine;

namespace Jeomseon.Unity.EditorToolkit.ScriptableObjects
{
    [CreateAssetMenu(fileName = "SpriteReference", menuName = "Scriptable Object/Sprite Reference")]
    public sealed class SpriteReference : ScriptableObject
    {
        [field: SerializeField] public Sprite Sprite { get; private set; }
    }
}
