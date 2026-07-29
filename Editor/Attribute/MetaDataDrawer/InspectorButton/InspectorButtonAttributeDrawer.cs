#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Jeomseon.Attribute.Editor
{
    using UnityEditorObjectEditor = UnityEditor.Editor;

    /// <summary>
    /// InspectorButtonAttribute가 지정된 메서드의 버튼을 기본 인스펙터 본문 아래에 그립니다.
    /// 자체 CustomEditor를 사용하는 타입에서는 OnInspectorGUI 마지막에 Draw를 직접 호출할 수 있습니다.
    /// </summary>
    public static class InspectorButtonGUI
    {
        private static readonly Dictionary<Type, IReadOnlyList<ButtonMethod>> Cache = new();

        public static void Draw(UnityEditorObjectEditor editor)
        {
            if (editor == null || editor.target == null)
                return;

            Type targetType = editor.target.GetType();
            if (!Cache.TryGetValue(targetType, out IReadOnlyList<ButtonMethod> methods))
            {
                methods = CollectMethods(targetType);
                Cache[targetType] = methods;
            }

            if (methods.Count == 0)
                return;

            EditorGUILayout.Space();
            using (new EditorGUILayout.HorizontalScope())
            {
                foreach (ButtonMethod item in methods)
                {
                    if (GUILayout.Button(item.Label))
                        InvokeForAllTargets(editor, item.Method);
                }
            }
        }

        private static IReadOnlyList<ButtonMethod> CollectMethods(Type type)
        {
            const BindingFlags Flags =
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            List<ButtonMethod> result = new();
            foreach (MethodInfo method in type.GetMethods(Flags))
            {
                InspectorButtonAttribute attribute = method.GetCustomAttribute<InspectorButtonAttribute>();
                if (attribute == null)
                    continue;

                if (method.GetParameters().Length != 0)
                {
                    Debug.LogWarning(
                        $"[{nameof(InspectorButtonGUI)}] 매개변수가 있는 메서드는 지원하지 않습니다: " +
                        $"{type.FullName}.{method.Name}");
                    continue;
                }

                string label = string.IsNullOrWhiteSpace(attribute.ButtonName)
                    ? ObjectNames.NicifyVariableName(method.Name)
                    : attribute.ButtonName;

                result.Add(new ButtonMethod(label, method));
            }

            return result;
        }

        private static void InvokeForAllTargets(UnityEditorObjectEditor editor, MethodInfo method)
        {
            foreach (UnityEngine.Object target in editor.targets)
            {
                try
                {
                    Undo.RecordObject(target, $"Invoke {method.Name}");
                    method.Invoke(target, null);
                    EditorUtility.SetDirty(target);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(target);
                }
                catch (TargetInvocationException exception)
                {
                    Debug.LogException(exception.InnerException ?? exception, target);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception, target);
                }
            }
        }

        private readonly struct ButtonMethod
        {
            public ButtonMethod(string label, MethodInfo method)
            {
                Label = label;
                Method = method;
            }

            public string Label { get; }
            public MethodInfo Method { get; }
        }
    }

    /// <summary>
    /// 다른 CustomEditor가 없는 MonoBehaviour에만 적용되는 공식 fallback Editor입니다.
    /// Unity 내부 InspectorWindow 구조를 리플렉션하지 않으면서 본문 하단 위치를 보장합니다.
    /// </summary>
    [CustomEditor(typeof(MonoBehaviour), true, isFallback = true)]
    [CanEditMultipleObjects]
    internal sealed class InspectorButtonMonoBehaviourEditor : UnityEditorObjectEditor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            InspectorButtonGUI.Draw(this);
        }
    }

    /// <summary>
    /// 다른 CustomEditor가 없는 ScriptableObject에만 적용되는 공식 fallback Editor입니다.
    /// </summary>
    [CustomEditor(typeof(ScriptableObject), true, isFallback = true)]
    [CanEditMultipleObjects]
    internal sealed class InspectorButtonScriptableObjectEditor : UnityEditorObjectEditor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            InspectorButtonGUI.Draw(this);
        }
    }
}
#endif
