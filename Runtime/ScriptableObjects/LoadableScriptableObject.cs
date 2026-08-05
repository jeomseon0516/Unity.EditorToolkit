using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jeomseon.ScriptableObjects
{
    [System.Serializable]
    public sealed class LoadableScriptableObject<T> where T : UnityEngine.ScriptableObject
    {
        public IReadOnlyList<T> ScriptableObjects => _scriptableObjects;
        public int Count => _scriptableObjects.Count;

        [SerializeField, HideInInspector]
        private List<T> _scriptableObjects = new();

        public T this[int index] => _scriptableObjects[index];
    }
}
