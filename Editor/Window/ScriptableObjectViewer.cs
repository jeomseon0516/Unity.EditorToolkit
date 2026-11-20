#if UNITY_EDITOR
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using Jeomseon.Unity.EditorToolkit.Editor.ScriptableObjects.Editor;

namespace Jeomseon.Unity.EditorToolkit.Editor.Window
{
    internal readonly struct ScrollBuildOption
    {
        public Type ScriptableObjectType { get; }
        public Type ScrollViewType { get; }
        public string Label { get; }

        public ScrollBuildOption(Type scriptableObjectType, string label)
        {
            ScriptableObjectType = scriptableObjectType;
            ScrollViewType = null;
            Label = label;
        }

        public ScrollBuildOption(Type scriptableObjectType, Type scrollViewType, string label) : this(scriptableObjectType, label)
        {
            ScrollViewType = scrollViewType;
        }
    }

    internal abstract class ScriptableObjectViewer : EditorWindow
    {
        // VisualElement는 생성자·필드 초기화에서 만들 수 없으므로 CreateGUI에서 생성한다.
        private ScriptableObjectInspector _inspector;
        private readonly List<ScriptableObjectScrollView> _scrollViews = new();
        private ToolbarButton _selectedButton = null;
        private Color _originalColor = ColorUtility.TryParseHtmlString("#505050", out Color color) ? color : Color.grey;

        protected abstract ScrollBuildOption[] GetTargetScriptableObjectType();

        protected void OnDisable()
        {
            _inspector?.SetTarget(null);
            _scrollViews.ForEach(view => view.OnDisable());
        }

        protected void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            _inspector = new ScriptableObjectInspector();
            _scrollViews.Clear();
            Toolbar toolbar = new()
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    backgroundColor = ColorUtility.TryParseHtmlString("#353535", out Color bgColor) ? bgColor : Color.gray,
                    paddingLeft = 1,
                    paddingRight = 1,
                    paddingTop = 1,
                    paddingBottom = 1,
                    height = 36,
                    maxHeight = 36,
                    flexGrow = 1,
                    borderBottomColor = Color.white,
                }
            };

            VisualElement contentElement = new()
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    flexGrow = 1,
                    flexShrink = 1,
                    borderBottomColor = Color.black,
                    borderTopColor = Color.black,
                    borderLeftColor = Color.black,
                    borderRightColor = Color.black,
                    borderBottomWidth = 1,
                    borderTopWidth = 1,
                    borderLeftWidth = 1,
                    borderRightWidth = 1
                },
            };

            foreach ((ToolbarButton, ScriptableObjectScrollView) buttonOption in GetTargetScriptableObjectType()
                .Where(option => option.ScriptableObjectType.IsSubclassOf(typeof(ScriptableObject)))
                .Select(GetButtonOption))
            {
                SetToolbarButton(buttonOption.Item1);
                toolbar.Add(buttonOption.Item1);
                buttonOption.Item2.name = "SOScrollView";
                buttonOption.Item1.clicked += () =>
                {
                    buttonOption.Item2.OnEnable();
                    _inspector.SetTarget(null);
                    VisualElement scrollView = root.Q<ScriptableObjectScrollView>("SOScrollView");
                    scrollView?.RemoveFromHierarchy();

                    contentElement.Insert(0, buttonOption.Item2);
                };
                buttonOption.Item2.OnSelectObject += scriptableObject => _inspector.SetTarget(scriptableObject.ScriptableObject);
                _scrollViews.Add(buttonOption.Item2);
            }

            var inspectorScrollView = new ScrollView { style = { flexGrow = 1 } };
            inspectorScrollView.Add(_inspector);
            contentElement.Add(inspectorScrollView);

            root.Add(toolbar);
            root.Add(contentElement);

            static (ToolbarButton, ScriptableObjectScrollView) GetButtonOption(ScrollBuildOption buildOption)
            {
                (ToolbarButton, ScriptableObjectScrollView) buttonOption;

                buttonOption.Item1 = new()
                {
                    text = buildOption.Label
                };

                buttonOption.Item2 = buildOption.ScrollViewType is null || !buildOption.ScrollViewType.IsSubclassOf(typeof(ScriptableObjectScrollView)) ?
                    new() :
                    (Activator.CreateInstance(buildOption.ScrollViewType) as ScriptableObjectScrollView)!;

                buttonOption.Item2.AddRange(ScriptableObjectHandler.LoadAllScriptableObjects(buildOption.ScriptableObjectType));
                buttonOption.Item2.Initialize();

                return buttonOption;
            }

            void SetToolbarButton(ToolbarButton button)
            {
                button.style.backgroundColor = _originalColor;
                button.style.unityTextAlign = TextAnchor.MiddleCenter;
                button.style.flexGrow = 1;
                button.style.marginRight = 0.25f;
                button.style.marginLeft = 0.25f;
                button.style.height = 30;
                button.clicked += () =>
                {
                    if (_selectedButton is not null)
                    {
                        _selectedButton.style.backgroundColor = _originalColor;
                    }

                    _selectedButton = button;
                    if (ColorUtility.TryParseHtmlString("#44577B", out Color btnColor))
                    {
                        _selectedButton.style.backgroundColor = btnColor;
                    }
                };
            }
        }
    }
}
#endif
