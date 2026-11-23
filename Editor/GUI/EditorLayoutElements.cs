#if UNITY_EDITOR
using System;
using UnityEngine.UIElements;

namespace Jeomseon.Unity.EditorToolkit.Editor.GUI
{
    /// <summary>
    /// <see cref="EditorGUILayoutActions"/>의 UI Toolkit판입니다.
    /// IMGUI의 Begin/End 세로 그룹 대신 내용을 채운 컨테이너 <see cref="VisualElement"/>를 반환합니다.
    /// <c>CreateGUI()</c>/<c>CreateInspectorGUI()</c>/<c>CreatePropertyGUI()</c> 안에서 호출해야 합니다.
    /// </summary>
    public static class EditorLayoutElements
    {
        /// <summary>
        /// <see cref="EditorGUILayoutActions.ActionEditorVertical(Action, UnityEngine.GUILayoutOption[])"/>에 대응합니다.
        /// 세로 방향 컨테이너를 만들고 <paramref name="buildContent"/>로 자식을 채웁니다.
        /// </summary>
        public static VisualElement CreateVertical(Action<VisualElement> buildContent)
        {
            var container = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Column
                }
            };

            buildContent?.Invoke(container);
            return container;
        }

        /// <summary>
        /// <see cref="EditorGUILayoutActions.ActionEditorVertical(Action, UnityEngine.GUIStyle, UnityEngine.GUILayoutOption[])"/>에
        /// <c>GUI.skin.box</c>를 전달한 경우에 대응합니다. 테두리가 있는 <see cref="Box"/>를 만들고 자식을 채웁니다.
        /// </summary>
        public static Box CreateVerticalBox(Action<VisualElement> buildContent)
        {
            var box = new Box();
            buildContent?.Invoke(box);
            return box;
        }

        /// <summary>
        /// <see cref="EditorGUILayoutActions.ActionEditorVerticalBox"/>에 대응합니다.
        /// <see cref="Box"/> 안에 세로 <see cref="ScrollView"/>를 두고 자식을 채웁니다. 스크롤 위치는
        /// <see cref="ScrollView"/>가 직접 보관합니다. <paramref name="height"/>가 0보다 크면 박스 높이를 고정합니다.
        /// </summary>
        public static Box CreateVerticalScrollBox(Action<VisualElement> buildContent, float height = 0f)
        {
            var box = new Box();
            if (height > 0f)
            {
                box.style.height = height;
            }

            var scrollView = new ScrollView(ScrollViewMode.Vertical)
            {
                style =
                {
                    flexGrow = 1
                }
            };
            box.Add(scrollView);

            buildContent?.Invoke(scrollView);
            return box;
        }
    }
}
#endif
