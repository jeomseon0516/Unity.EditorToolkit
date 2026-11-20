#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jeomseon.Unity.EditorToolkit.Editor.GUI
{
    using GUI = UnityEngine.GUI;

    public sealed class ToggleEnumerator<T>
    {
        public T ChooseTarget { get; private set; }
        public bool IsInitializeGUIStyle => _selectedGUIStyle is not null && _defaultGUIStyle is not null;
        public bool IsInitializeOnGetDataList => _onGetDataList is not null;

        private Func<IEnumerable<T>> _onGetDataList;
        private GUIStyle _selectedGUIStyle;
        private GUIStyle _defaultGUIStyle;

        public void InitializeGUIStyle(GUIStyle selectedGUIStyle, GUIStyle defaultGUIStyle)
        {
            _selectedGUIStyle = selectedGUIStyle;
            _defaultGUIStyle = defaultGUIStyle;
        }

        public void InitializeGUIStyle(Color32 selectedStyleColor, Color32 defaultStyleColor)
        {
            _selectedGUIStyle = new(GUI.skin.box);
            _defaultGUIStyle = new(GUI.skin.box);

            _selectedGUIStyle.normal.background = GUIStyleTexture.Create(selectedStyleColor, _selectedGUIStyle);
            _defaultGUIStyle.normal.background = GUIStyleTexture.Create(defaultStyleColor, _defaultGUIStyle);
        }

        public T SelectEnumeratedToggles(Func<T, string> onSelectedText, params GUILayoutOption[] options)
        {
            T selectingTarget = ChooseTarget;

            foreach (T target in _onGetDataList?.Invoke() ?? Enumerable.Empty<T>())
            {
                bool isTargeting = ChooseTarget?.Equals(target) ?? false;
                bool isSelected = GUILayout.Toggle(
                    isTargeting,
                    onSelectedText.Invoke(target),
                    isTargeting ? _selectedGUIStyle : _defaultGUIStyle,
                    options);

                selectingTarget = isSelected switch
                {
                    true when !isTargeting => target,
                    false when isTargeting => default,
                    _ => selectingTarget
                };
            }

            ChooseTarget = selectingTarget;
            return ChooseTarget;
        }

        /// <summary>
        /// <see cref="SelectEnumeratedToggles"/>의 UI Toolkit판입니다. 데이터 목록의 각 항목을 배경색으로
        /// 선택 상태를 표시하는 토글 버튼으로 나열한 컨테이너를 반환합니다. 선택된 항목을 다시 누르면 선택이
        /// 해제되어 <see cref="ChooseTarget"/>이 <c>default</c>가 됩니다. 데이터 목록은 생성 시점에 한 번 읽습니다.
        /// </summary>
        /// <param name="onSelectedText">항목의 표시 문자열을 반환합니다.</param>
        /// <param name="selectedStyleColor">선택된 항목의 배경색입니다.</param>
        /// <param name="defaultStyleColor">선택되지 않은 항목의 배경색입니다.</param>
        /// <param name="itemHeight">0보다 크면 각 항목의 높이를 고정합니다.</param>
        /// <param name="onChanged">선택이 바뀐 뒤 새 <see cref="ChooseTarget"/>으로 호출됩니다.</param>
        public VisualElement CreateEnumeratedToggles(
            Func<T, string> onSelectedText,
            Color32 selectedStyleColor,
            Color32 defaultStyleColor,
            float itemHeight = 0f,
            Action<T> onChanged = null)
        {
            var container = new VisualElement();
            var items = new List<(T Target, Button Button)>();
            Color selectedColor = selectedStyleColor;
            Color defaultColor = defaultStyleColor;

            foreach (T target in _onGetDataList?.Invoke() ?? Enumerable.Empty<T>())
            {
                T item = target;
                var button = new Button
                {
                    text = onSelectedText.Invoke(item)
                };

                if (itemHeight > 0f)
                {
                    button.style.height = itemHeight;
                }

                button.clicked += () =>
                {
                    bool isTargeting = ChooseTarget?.Equals(item) ?? false;
                    ChooseTarget = isTargeting ? default : item;
                    RefreshItems();
                    onChanged?.Invoke(ChooseTarget);
                };

                items.Add((item, button));
                container.Add(button);
            }

            RefreshItems();
            return container;

            void RefreshItems()
            {
                foreach ((T itemTarget, Button itemButton) in items)
                {
                    bool isTargeting = ChooseTarget?.Equals(itemTarget) ?? false;
                    itemButton.style.backgroundColor = isTargeting ? selectedColor : defaultColor;
                }
            }
        }

        public void SetOnGetDataList(Func<IEnumerable<T>> onGetDataList)
        {
            _onGetDataList ??= onGetDataList;
        }
    }
}
#endif