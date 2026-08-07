#if UNITY_EDITOR
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

namespace Jeomseon.ScriptableObjects.Editor
{
    using ScriptableObject = UnityEngine.ScriptableObject;

    [CustomPropertyDrawer(typeof(LoadableScriptableObject<>), true)]
    internal sealed class LoadableScriptableObjectDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty listProperty = property.FindPropertyRelative("_scriptableObjects")!;
            RefreshScriptableObjects(property, listProperty);

            return new ListView
            {
                headerTitle = property.displayName,
                showFoldoutHeader = true,
                showBorder = true,
                reorderable = true,
                showAddRemoveFooter = false,
                bindingPath = listProperty.propertyPath,
                makeItem = () =>
                {
                    var field = new ObjectField { objectType = typeof(ScriptableObject) };
                    field.SetEnabled(false);
                    return field;
                }
            };
        }

        private void RefreshScriptableObjects(SerializedProperty property, SerializedProperty listProperty)
        {
            Type listFieldType = fieldInfo.FieldType.GetElementType() ?? fieldInfo.FieldType.GetGenericArguments()[0];

            List<ScriptableObject> scriptableObjects = AssetDatabase
                .FindAssets($"t:{listFieldType.Name}")
                .Select(guid => AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), listFieldType))
                .OfType<ScriptableObject>()
                .ToList();

            listProperty.ClearArray();
            for (int i = 0; i < scriptableObjects.Count; i++)
            {
                listProperty.InsertArrayElementAtIndex(i);
                listProperty.GetArrayElementAtIndex(i).objectReferenceValue = scriptableObjects[i];
            }

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
