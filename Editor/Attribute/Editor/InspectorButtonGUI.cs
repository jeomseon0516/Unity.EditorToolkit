#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Jeomseon.Attribute.Editor
{
    using UnityEditorObjectEditor = UnityEditor.Editor;

    /// <summary>
    /// InspectorButtonAttribute가 지정된 메서드의 버튼을 인스펙터 본문 아래에 그립니다.
    /// Inspector Injection 백엔드와 자체 CustomEditor의 명시적 호출이 이 진입점을 함께 사용합니다.
    /// </summary>
    public static class InspectorButtonGUI
    {
        public static void Draw(UnityEditorObjectEditor editor)
        {
            if (editor == null || editor.target == null)
                return;

            InspectorButtonMethod[] methods =
                InspectorButtonMethodCache.Get(editor.target.GetType());
            if (methods.Length == 0)
                return;

            EditorGUILayout.Space();

            foreach (InspectorButtonMethod item in methods)
            {
                if (GUILayout.Button(item.Label))
                    InspectorButtonInvoker.InvokeForAllTargets(editor, item.Method);
            }
        }
    }
}
#endif
