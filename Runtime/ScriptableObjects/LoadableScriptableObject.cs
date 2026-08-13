using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Jeomseon.Unity.EditorToolkit.ScriptableObjects
{
    [System.Serializable]
    public sealed class LoadableScriptableObject<T> where T : UnityEngine.ScriptableObject
    {
        public IReadOnlyList<T> ScriptableObjects => scriptableObjects;
        public int Count => scriptableObjects.Count;

        [SerializeField, HideInInspector, FormerlySerializedAs("_scriptableObjects")]
        private List<T> scriptableObjects = new();

        public T this[int index] => scriptableObjects[index];
    }
}
