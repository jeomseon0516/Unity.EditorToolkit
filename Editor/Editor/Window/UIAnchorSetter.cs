#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Jeomseon.Editor.Window
{
    public class UIAnchorSetter : EditorWindow
    {
        [MenuItem("Jeomseon/Tool/UI Anchor Setter Tool")]
        private static void ShowWindow()
        {
            GetWindow<UIAnchorSetter>(nameof(UIAnchorSetter));
        }

        private RectTransform _selectedUI;

        private void CreateGUI()
        {
            var root = rootVisualElement;
            root.style.paddingLeft = 8;
            root.style.paddingRight = 8;
            root.style.paddingTop = 8;
            root.style.paddingBottom = 8;

            var title = new Label("Anchor Setter Tool");
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            root.Add(title);

            var objectField = new ObjectField("UI Object")
            {
                objectType = typeof(RectTransform),
                allowSceneObjects = true,
                value = _selectedUI
            };
            objectField.RegisterValueChangedCallback(evt => _selectedUI = evt.newValue as RectTransform);
            root.Add(objectField);

            root.Add(new Button(SetAnchors)
            {
                text = "Set Anchors for Child"
            });
        }

        private void SetAnchors()
        {
            if (!_selectedUI)
                return;

            var rects = _selectedUI
                .GetComponentsInChildren<RectTransform>(true)
                .Where(rt =>
                    !rt.GetComponent<Canvas>() &&
                    (!rt.parent || !rt.parent.GetComponent<LayoutGroup>()))
                .ToArray();

            Undo.RecordObjects(rects, "Set UI Anchors");

            foreach (RectTransform rectTransform in rects)
            {
                RectTransform parent = rectTransform.parent as RectTransform;
                if (!parent) continue;

                Vector2 newAnchorMin = new(
                    rectTransform.anchorMin.x + rectTransform.offsetMin.x / parent.rect.width,
                    rectTransform.anchorMin.y + rectTransform.offsetMin.y / parent.rect.height);

                Vector2 newAnchorMax = new(
                    rectTransform.anchorMax.x + rectTransform.offsetMax.x / parent.rect.width,
                    rectTransform.anchorMax.y + rectTransform.offsetMax.y / parent.rect.height);

                rectTransform.anchorMin = newAnchorMin;
                rectTransform.anchorMax = newAnchorMax;
                rectTransform.offsetMin = rectTransform.offsetMax = Vector2.zero;
            }

            EditorUtility.SetDirty(_selectedUI);
        }
    }
}
#endif
