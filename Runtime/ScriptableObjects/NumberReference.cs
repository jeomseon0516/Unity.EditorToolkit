using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jeomseon.Unity.EditorToolkit.ScriptableObjects
{
    public sealed class NumberReference : UnityEngine.ScriptableObject
    {
        [field: SerializeField] public int No { get; }
    }
}