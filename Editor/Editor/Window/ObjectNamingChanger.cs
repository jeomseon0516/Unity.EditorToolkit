#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Jeomseon.Collections;
using UnityEditor.UIElements;

namespace Jeomseon.Editor.Window
{
    public sealed class ObjectNamingChanger : EditorWindow
    {
        private enum NameChangeState : byte
        {
            OnlyCurrent,
            ChildAll
        }

        private enum ChangeMode : byte
        {
            Replace,
            Add
        }

        private enum AffixState : byte
        {
            Prefix,
            Suffix
        }

        private GameObject _targetObject;
        private ChangeMode _changeMode = ChangeMode.Replace;
        private NameChangeState _nameChangeState = NameChangeState.OnlyCurrent;
        private AffixState _affixState = AffixState.Prefix;
        private string _replaceTargetText = string.Empty;
        private string _replaceChangedTargetText = string.Empty;
        private string _affixText = string.Empty;
        private VisualElement _content;
        private VisualElement _replaceContent;
        private VisualElement _addContent;

        [MenuItem("Jeomseon/Tool/Object Naming Changer")]
        private static void ShowWindow()
        {
            ObjectNamingChanger objectNamingChanger = CreateWindow<ObjectNamingChanger>();
            objectNamingChanger.Show();
        }

        private void CreateGUI()
        {
            var root = rootVisualElement;
            root.style.paddingLeft = 8;
            root.style.paddingRight = 8;
            root.style.paddingTop = 8;
            root.style.paddingBottom = 8;

            var targetField = new ObjectField("Target Object")
            {
                objectType = typeof(GameObject),
                allowSceneObjects = true,
                value = _targetObject
            };
            targetField.RegisterValueChangedCallback(evt =>
            {
                _targetObject = evt.newValue as GameObject;
                RefreshContent();
            });
            root.Add(targetField);

            _content = new VisualElement();
            root.Add(_content);

            var options = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row
                }
            };

            var modeField = new EnumField("Change Mode", _changeMode);
            modeField.style.flexGrow = 1;
            modeField.RegisterValueChangedCallback(evt =>
            {
                _changeMode = (ChangeMode)evt.newValue;
                RefreshModeContent();
            });
            options.Add(modeField);

            var stateField = new EnumField("Change Scope", _nameChangeState);
            stateField.style.flexGrow = 1;
            stateField.RegisterValueChangedCallback(evt =>
                _nameChangeState = (NameChangeState)evt.newValue);
            options.Add(stateField);
            _content.Add(options);

            _replaceContent = CreateReplaceContent();
            _addContent = CreateAddContent();
            _content.Add(_replaceContent);
            _content.Add(_addContent);

            RefreshContent();
        }

        private VisualElement CreateReplaceContent()
        {
            var content = new VisualElement();

            var targetField = new TextField("Target Text")
            {
                value = _replaceTargetText
            };
            targetField.RegisterValueChangedCallback(evt => _replaceTargetText = evt.newValue);
            content.Add(targetField);

            var changedField = new TextField("Changed Text");
            changedField.value = _replaceChangedTargetText;
            changedField.RegisterValueChangedCallback(evt => _replaceChangedTargetText = evt.newValue);
            content.Add(changedField);

            content.Add(new Button(ReplaceNames)
            {
                text = "Replace"
            });

            return content;
        }

        private VisualElement CreateAddContent()
        {
            var content = new VisualElement();

            var affixField = new EnumField("Affix", _affixState);
            affixField.RegisterValueChangedCallback(evt => _affixState = (AffixState)evt.newValue);
            content.Add(affixField);

            var textField = new TextField("Affix Text");
            textField.value = _affixText;
            textField.RegisterValueChangedCallback(evt => _affixText = evt.newValue);
            content.Add(textField);

            content.Add(new Button(AddAffix)
            {
                text = "Affix"
            });

            return content;
        }

        private void RefreshContent()
        {
            _content.style.display = _targetObject ? DisplayStyle.Flex : DisplayStyle.None;
            if (_targetObject)
                RefreshModeContent();
        }

        private void RefreshModeContent()
        {
            _replaceContent.style.display = _changeMode == ChangeMode.Replace
                ? DisplayStyle.Flex
                : DisplayStyle.None;
            _addContent.style.display = _changeMode == ChangeMode.Add
                ? DisplayStyle.Flex
                : DisplayStyle.None;
        }

        private void ReplaceNames()
        {
            if (!string.IsNullOrEmpty(_replaceTargetText))
                ApplyToTarget(go => go.name = go.name.Replace(_replaceTargetText, _replaceChangedTargetText));
        }

        private void AddAffix()
        {
            if (string.IsNullOrEmpty(_affixText))
                return;

            switch (_affixState)
            {
                case AffixState.Prefix:
                    ApplyToTarget(go => go.name = $"{_affixText}_{go.name}");
                    break;
                case AffixState.Suffix:
                    ApplyToTarget(go => go.name += $"_{_affixText}");
                    break;
            }
        }

        private void ApplyToTarget(Action<GameObject> action)
        {
            switch (_nameChangeState)
            {
                case NameChangeState.OnlyCurrent:
                    action?.Invoke(_targetObject);
                    break;
                case NameChangeState.ChildAll:
                    if (action is null) return;
                    _targetObject
                        .GetComponentsInChildren<Transform>()
                        .Select(transform => transform.gameObject)
                        .ForEach(action);
                    break;
            }
        }
    }
}
#endif
