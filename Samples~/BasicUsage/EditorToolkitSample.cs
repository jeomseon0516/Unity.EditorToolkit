#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Jeomseon.Unity.EditorToolkit.Editor;
using Jeomseon.Unity.EditorToolkit.Editor.Extensions;
using Jeomseon.Unity.EditorToolkit.Editor.GUI;
using UnityEngine.Serialization;

namespace Jeomseon.Samples.EditorToolkit
{
    internal sealed class EditorToolkitSampleData : ScriptableObject
    {
        [SerializeField, FormerlySerializedAs("_title")] private string title = "Sample";
        [SerializeField, FormerlySerializedAs("_amount")] private int amount = 1;
        [SerializeField, FormerlySerializedAs("_isEnabled")] private bool isEnabled = true;
    }

    public sealed class EditorToolkitSample : EditorWindow
    {
        private static readonly string[] _toggleOptions = { "Apple", "Banana", "Cherry" };
        private static readonly Dictionary<string, int> _dropdownOptions = new()
        {
            { "Low", 0 },
            { "Medium", 1 },
            { "High", 2 }
        };

        private ToggleEnumerator<string> _toggleEnumerator;
        private EditorDropdownController<int> _dropdownController;
        private int _dropdownValue;
        private Vector2 _boxScrollPosition;
        private GUIStyle _boxStyle;
        private List<Type> _discoveredTypes;
        private SerializedObject _sampleSerializedObject;

        [MenuItem("Jeomseon/Samples/Editor Toolkit Sample")]
        private static void ShowWindow()
        {
            GetWindow<EditorToolkitSample>(nameof(EditorToolkitSample));
        }

        private void OnEnable()
        {
            _toggleEnumerator = new ToggleEnumerator<string>();
            _toggleEnumerator.SetOnGetDataList(() => _toggleOptions);

            _dropdownController = new EditorDropdownController<int>("Priority", _dropdownOptions);

            EditorToolkitSampleData sampleData = CreateInstance<EditorToolkitSampleData>();
            sampleData.hideFlags = HideFlags.DontSave;
            _sampleSerializedObject = new SerializedObject(sampleData);
        }

        private void OnDisable()
        {
            if (_sampleSerializedObject?.targetObject)
            {
                DestroyImmediate(_sampleSerializedObject.targetObject);
            }
        }

        private void OnGUI()
        {
            // GUI.skin은 OnGUI 안에서만 접근 가능하므로, 초기화를 OnEnable이 아닌 여기서 1회만 수행합니다.
            if (!_toggleEnumerator.IsInitializeGUIStyle)
            {
                _toggleEnumerator.InitializeGUIStyle(new Color32(60, 120, 200, 255), new Color32(60, 60, 60, 255));
            }

            EditorGUILayout.LabelField("1. ToggleEnumerator<T> + GUIStyleTexture", EditorStyles.boldLabel);
            string selected = _toggleEnumerator.SelectEnumeratedToggles(option => option, GUILayout.Height(24));
            EditorGUILayout.LabelField("Selected", selected ?? "(none)");

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("2. EditorDropdownController<T>", EditorStyles.boldLabel);
            _dropdownController.Dropdown(value => _dropdownValue = value);
            EditorGUILayout.LabelField("Selected Value", _dropdownValue.ToString());

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("3. EditorGUILayoutActions.ActionEditorVerticalBox", EditorStyles.boldLabel);
            _boxStyle ??= new GUIStyle(UnityEngine.GUI.skin.box);
            EditorGUILayoutActions.ActionEditorVerticalBox(_boxStyle, ref _boxScrollPosition, () =>
            {
                for (int i = 0; i < 10; i++)
                {
                    EditorGUILayout.LabelField($"Scrollable line {i}");
                }
            }, GUILayout.Height(80));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("4. EditorGUILayoutActions.ActionEditorVertical", EditorStyles.boldLabel);
            EditorGUILayoutActions.ActionEditorVertical(() =>
            {
                EditorGUILayout.HelpBox("This block is wrapped by ActionEditorVertical.", MessageType.Info);
            }, UnityEngine.GUI.skin.box);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("5. EditorTypeDiscovery", EditorStyles.boldLabel);
            if (GUILayout.Button("Discover Concrete Component Types"))
            {
                _discoveredTypes = EditorTypeDiscovery.GetConcreteTypesDerivedFrom<Component>().ToList();
            }
            if (_discoveredTypes is not null)
            {
                EditorGUILayout.LabelField($"Found {_discoveredTypes.Count} concrete Component type(s). First 5:");
                foreach (Type type in _discoveredTypes.Take(5))
                {
                    EditorGUILayout.LabelField($"- {type.FullName}");
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("6. SerializedPropertyExtensions.GetPropertyType()", EditorStyles.boldLabel);
            DrawResolvedPropertyType("title");
            DrawResolvedPropertyType("amount");
            DrawResolvedPropertyType("isEnabled");
        }

        private void DrawResolvedPropertyType(string fieldName)
        {
            SerializedProperty property = _sampleSerializedObject.FindProperty(fieldName);
            Type resolvedType = property?.GetPropertyType();
            EditorGUILayout.LabelField(fieldName, resolvedType?.Name ?? "(unresolved)");
        }
    }
}
#endif
