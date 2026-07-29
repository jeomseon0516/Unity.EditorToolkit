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
    /// InspectorButtonAttribute가 지정된 메서드의 버튼을 인스펙터 본문 아래에 그립니다.
    /// Inspector Injection 백엔드와 자체 CustomEditor의 명시적 호출이 이 진입점을 함께 사용합니다.
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

    internal sealed class InspectorButtonInjectedDrawer : IInspectorInjectedDrawer
    {
        public void OnEnable(UnityEditorObjectEditor editor)
        {
        }

        public void OnInspectorGUI(UnityEditorObjectEditor editor)
        {
            InspectorButtonGUI.Draw(editor);
        }

        public void Dispose()
        {
        }
    }
}
#endif
