#if UNITY_EDITOR
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace Jeomseon.Unity.EditorToolkit.Editor.GUI
{
    public sealed class EditorDropdownController<T>
    {
        public IReadOnlyDictionary<string, T> Options => _options;
        public KeyValuePair<string, T> SelectedItem { get; private set; } = default;

        private readonly string _label = string.Empty;
        private readonly T _default = default;
        private readonly Dictionary<string, T> _options;

        public void Dropdown(Action<T> onChangeValue)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(_label, GUILayout.ExpandWidth(true));
                if (EditorGUILayout.DropdownButton(
                    new(string.IsNullOrEmpty(SelectedItem.Key) ? _label : SelectedItem.Key),
                    FocusType.Keyboard))
                {
                    GenericMenu menu = CreateMenu(onChangeValue);
                    menu.ShowAsContext();
                }
            }
        }

        /// <summary>
        /// <see cref="Dropdown"/>의 UI Toolkit판입니다. 라벨과 드롭다운 버튼을 가로로 배치한 행을 반환합니다.
        /// 버튼을 누르면 IMGUI판과 같은 <see cref="GenericMenu"/>를 버튼 아래에 엽니다.
        /// </summary>
        public VisualElement CreateDropdown(Action<T> onChangeValue)
        {
            var row = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row
                }
            };

            var label = new Label(_label)
            {
                style =
                {
                    flexGrow = 1,
                    flexBasis = 0,
                    unityTextAlign = TextAnchor.MiddleLeft
                }
            };

            var button = new Button
            {
                style =
                {
                    flexGrow = 1,
                    flexBasis = 0
                }
            };

            button.clicked += () =>
            {
                GenericMenu menu = CreateMenu(value =>
                {
                    RefreshButtonText();
                    onChangeValue.Invoke(value);
                });
                menu.DropDown(button.worldBound);
            };

            RefreshButtonText();
            row.Add(label);
            row.Add(button);
            return row;

            void RefreshButtonText()
            {
                button.text = string.IsNullOrEmpty(SelectedItem.Key) ? _label : SelectedItem.Key;
            }
        }

        private GenericMenu CreateMenu(Action<T> onChangeValue)
        {
            GenericMenu menu = new();

            foreach (KeyValuePair<string, T> optionKvp in _options)
            {
                menu.AddItem(
                    new GUIContent(optionKvp.Key),
                    SelectedItem.Key == optionKvp.Key,
                    () =>
                    {
                        SelectedItem = optionKvp;
                        onChangeValue.Invoke(optionKvp.Value);
                    });
            }

            if (_default is not null && !_default.Equals(default))
            {
                menu.AddItem(
                    new GUIContent("NONE"),
                    false,
                    () =>
                    {
                        SelectedItem = default;
                        onChangeValue.Invoke(_default);
                    });
            }

            return menu;
        }

        public void SetSelectedItem(T value)
        {
            SelectedItem = Options.FirstOrDefault(kvp => kvp.Value.Equals(value));
        }

        private EditorDropdownController() { }
        public EditorDropdownController(string label, Dictionary<string, T> options, T defaultValue = default)
        {
            _label = label;
            _default = defaultValue;
            _options = options;
            SelectedItem = _options.FirstOrDefault();
        }
    }
}
#endif