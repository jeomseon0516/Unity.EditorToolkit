using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jeomseon.Unity.EditorToolkit.Editor.Window
{
    public sealed class ScriptableObjectInspector : VisualElement
    {
        private ScriptableObject _target;

        public ScriptableObjectInspector()
        {
            RegisterCallback<AttachToPanelEvent>(_ => Rebuild());
        }

        public void SetTarget(ScriptableObject target)
        {
            if (ReferenceEquals(_target, target))
                return;

            _target = target;
            Rebuild();
        }

        private void Rebuild()
        {
            Clear();
            if (panel != null && _target != null)
                Add(new InspectorElement(_target));
        }
    }
}
