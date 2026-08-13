#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        public void SetOnGetDataList(Func<IEnumerable<T>> onGetDataList)
        {
            _onGetDataList ??= onGetDataList;
        }
    }
}
#endif