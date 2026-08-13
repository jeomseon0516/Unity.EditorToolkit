#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;

namespace Jeomseon.Unity.EditorToolkit.Editor.GUI
{
    public static class EditorGUILayoutActions
    {
        public static void ActionEditorVertical(Action action, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginVertical(options);
            action.Invoke();
            EditorGUILayout.EndVertical();
        }

        public static void ActionEditorVertical(Action action, GUIStyle guiStyle, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginVertical(guiStyle, options);
            action.Invoke();
            EditorGUILayout.EndVertical();
        }

        public static void ActionEditorVerticalBox(GUIStyle guiStyle, ref Vector2 scrollPosition, Action action, params GUILayoutOption[] options)
        {
            using EditorGUILayout.VerticalScope verticalScope = new(guiStyle, options);
            using EditorGUILayout.ScrollViewScope scrollScope = new(scrollPosition);
            scrollPosition = scrollScope.scrollPosition;
            action.Invoke();
        }
    }
}
#endif
