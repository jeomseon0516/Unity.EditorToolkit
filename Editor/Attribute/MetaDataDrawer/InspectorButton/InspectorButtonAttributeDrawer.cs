#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace Jeomseon.Attribute.Editor
{
    using Editor = UnityEditor.Editor;

    /// <summary>
    /// InspectorButtonAttribute를 처리하는 전역 드로어입니다.
    /// Unity 내부 InspectorWindow 구현을 리플렉션하지 않고
    /// 오래전부터 제공되는 Editor.finishedDefaultHeaderGUI 공식 API만 사용합니다.
    /// </summary>
    [InitializeOnLoad]
    internal static class InspectorButtonHeaderDrawer
    {
        // 타입별로 버튼 메서드 캐시 (라벨, 메서드)
        private static readonly Dictionary<Type, List<(string label, MethodInfo method)>> _cache
            = new();

        static InspectorButtonHeaderDrawer()
        {
            // TODO(UX): 헤더가 아닌 본문 하단 배치가 필요해지면 CustomEditor를 강제하지 말고
            // 공식 Editor 확장 지점을 제공하는 별도 opt-in 베이스 Editor 방식을 검토해야 합니다.
            Editor.finishedDefaultHeaderGUI += OnFinishedDefaultHeaderGUI;
        }

        private static void OnFinishedDefaultHeaderGUI(Editor editor)
        {
            if (editor == null)
                return;

            // 대상이 MonoBehaviour 또는 ScriptableObject일 때만 처리
            var target = editor.target;
            if (target is not MonoBehaviour && target is not ScriptableObject)
                return;

            Type targetType = target.GetType();

            if (!_cache.TryGetValue(targetType, out var buttonMethods))
            {
                buttonMethods = CollectButtonMethods(targetType);
                _cache[targetType] = buttonMethods;
            }

            if (buttonMethods == null || buttonMethods.Count == 0)
                return;

            // 기본 헤더와의 간격 살짝
            GUILayout.Space(4f);

            // 한 줄에 버튼들 배치
            using (new EditorGUILayout.HorizontalScope())
            {
                foreach (var (label, method) in buttonMethods)
                {
                    if (GUILayout.Button(label))
                    {
                        InvokeForAllTargets(editor, method);
                    }
                }
            }
        }

        /// <summary>
        /// 타입에서 InspectorButtonAttribute가 달린 메서드들을 수집.
        /// </summary>
        private static List<(string label, MethodInfo method)> CollectButtonMethods(Type type)
        {
            var list = new List<(string, MethodInfo)>();

            const BindingFlags flags =
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            var methods = type.GetMethods(flags);

            foreach (var method in methods)
            {
                var attr = method.GetCustomAttribute<InspectorButtonAttribute>();
                if (attr == null)
                    continue;

                // 파라미터 있는 메서드는 무시 (필요하면 확장 가능)
                if (method.GetParameters().Length > 0)
                    continue;

                string label = string.IsNullOrEmpty(attr.ButtonName)
                    ? method.Name
                    : attr.ButtonName;

                list.Add((label, method));
            }

            return list;
        }

        /// <summary>
        /// 멀티 오브젝트 선택 시, 모든 target에 대해 메서드 호출.
        /// </summary>
        private static void InvokeForAllTargets(Editor editor, MethodInfo method)
        {
            var targets = editor.targets;
            foreach (var t in targets)
            {
                try
                {
                    method.Invoke(t, null);

                    // 변경사항 반영이 필요하면 MarkDirty
                    if (t is UnityEngine.Object obj)
                    {
                        EditorUtility.SetDirty(obj);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }
    }
}
#endif
