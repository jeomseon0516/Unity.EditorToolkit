#if UNITY_EDITOR
using UnityEngine;

namespace Jeomseon.Editor.Window
{
    using Editor = UnityEditor.Editor;

    internal class CustomInspectorEditorWindow
    {
        private Editor _editor = null;
        private Object _currentTarget = null;

        public void OnInspectorGUI(Object target)
        {
            if (target == null)
            {
                return;
            }

            if (_editor == null || _currentTarget != target)
            {
                CreateInspectorEditor(target);
            }

            if (_editor != null)
            {
                _editor.OnInspectorGUI();
            }
        }

        private void CreateInspectorEditor(Object target)
        {
            DestroyInspectorEditor();

            _currentTarget = target;
            _editor = Editor.CreateEditor(target);
        }

        public void OnDisable()
        {
            DestroyInspectorEditor();
        }

        private void DestroyInspectorEditor()
        {
            if (_editor != null)
            {
                Object.DestroyImmediate(_editor);
                _editor = null;
                _currentTarget = null;
            }
        }
    }
}
#endif