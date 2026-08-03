#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Jeomseon.Attribute.Editor
{
    using UnityEditorObjectEditor = UnityEditor.Editor;

    /// <summary>
    /// InspectorButton 호출 대상 검증, Undo 및 Unity 객체 변경 기록을 담당합니다.
    /// </summary>
    internal static class InspectorButtonInvoker
    {
        public static void InvokeForAllTargets(
            UnityEditorObjectEditor editor,
            MethodInfo method)
        {
            if (editor == null || method == null)
                return;

            Undo.IncrementCurrentGroup();
            int group = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName($"Invoke {method.Name}");

            try
            {
                foreach (UnityEngine.Object target in editor.targets)
                {
                    EditorMethodInvocation invocation =
                        new(target, method);
                    if (!EditorMethodInvoker.CanInvoke(invocation))
                        continue;

                    Undo.RecordObject(target, $"Invoke {method.Name}");
                    EditorMethodInvoker.TryInvoke(invocation);
                }
            }
            finally
            {
                Undo.CollapseUndoOperations(group);
            }
        }
    }
}
#endif
