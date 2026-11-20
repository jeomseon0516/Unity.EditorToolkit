#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
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

        private void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            root.Add(CreateHeader("1. ToggleEnumerator<T>.CreateEnumeratedToggles"));
            Label selectedLabel = null;
            root.Add(_toggleEnumerator.CreateEnumeratedToggles(
                option => option,
                new Color32(60, 120, 200, 255),
                new Color32(60, 60, 60, 255),
                24f,
                selected => selectedLabel.text = selected ?? "(none)"));
            root.Add(CreateLabelValueRow("Selected", _toggleEnumerator.ChooseTarget ?? "(none)", out selectedLabel));

            root.Add(CreateSpace());
            root.Add(CreateHeader("2. EditorDropdownController<T>.CreateDropdown"));
            Label dropdownValueLabel = null;
            root.Add(_dropdownController.CreateDropdown(value =>
            {
                _dropdownValue = value;
                dropdownValueLabel.text = _dropdownValue.ToString();
            }));
            root.Add(CreateLabelValueRow("Selected Value", _dropdownValue.ToString(), out dropdownValueLabel));

            root.Add(CreateSpace());
            root.Add(CreateHeader("3. EditorLayoutElements.CreateVerticalScrollBox"));
            root.Add(EditorLayoutElements.CreateVerticalScrollBox(box =>
            {
                for (int i = 0; i < 10; i++)
                {
                    box.Add(new Label($"Scrollable line {i}"));
                }
            }, 80f));

            root.Add(CreateSpace());
            root.Add(CreateHeader("4. EditorLayoutElements.CreateVerticalBox"));
            root.Add(EditorLayoutElements.CreateVerticalBox(box =>
            {
                box.Add(new HelpBox("This block is wrapped by CreateVerticalBox.", HelpBoxMessageType.Info));
            }));

            root.Add(CreateSpace());
            root.Add(CreateHeader("5. EditorTypeDiscovery"));
            var discoveredTypesContainer = new VisualElement();
            root.Add(new Button(() =>
            {
                _discoveredTypes = EditorTypeDiscovery.GetConcreteTypesDerivedFrom<Component>().ToList();
                RefreshDiscoveredTypes(discoveredTypesContainer);
            })
            {
                text = "Discover Concrete Component Types"
            });
            root.Add(discoveredTypesContainer);
            RefreshDiscoveredTypes(discoveredTypesContainer);

            root.Add(CreateSpace());
            root.Add(CreateHeader("6. SerializedPropertyExtensions.GetPropertyType()"));
            root.Add(CreateResolvedPropertyTypeRow("title"));
            root.Add(CreateResolvedPropertyTypeRow("amount"));
            root.Add(CreateResolvedPropertyTypeRow("isEnabled"));
        }

        private void RefreshDiscoveredTypes(VisualElement container)
        {
            container.Clear();
            if (_discoveredTypes is null)
            {
                return;
            }

            container.Add(new Label($"Found {_discoveredTypes.Count} concrete Component type(s). First 5:"));
            foreach (Type type in _discoveredTypes.Take(5))
            {
                container.Add(new Label($"- {type.FullName}"));
            }
        }

        private VisualElement CreateResolvedPropertyTypeRow(string fieldName)
        {
            SerializedProperty property = _sampleSerializedObject.FindProperty(fieldName);
            Type resolvedType = property?.GetPropertyType();
            return CreateLabelValueRow(fieldName, resolvedType?.Name ?? "(unresolved)", out _);
        }

        private static Label CreateHeader(string text)
        {
            return new Label(text)
            {
                style =
                {
                    unityFontStyleAndWeight = FontStyle.Bold
                }
            };
        }

        private static VisualElement CreateSpace()
        {
            // EditorGUILayout.Space()의 기본 간격(6px)과 맞춥니다.
            return new VisualElement
            {
                style =
                {
                    height = 6
                }
            };
        }

        private static VisualElement CreateLabelValueRow(string label, string value, out Label valueLabel)
        {
            var row = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row
                }
            };

            row.Add(new Label(label)
            {
                style =
                {
                    width = EditorGUIUtility.labelWidth,
                    flexShrink = 0
                }
            });

            valueLabel = new Label(value);
            row.Add(valueLabel);
            return row;
        }
    }
}
#endif
