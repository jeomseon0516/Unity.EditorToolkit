#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jeomseon.Editor.Tool
{
    [CustomEditor(typeof(IconCreatorPreset))]
    internal sealed class IconCreatorPresetEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            InspectorElement.FillDefaultInspector(root, serializedObject, this);

            var warnings = new VisualElement();
            root.Add(warnings);

            RefreshWarnings(warnings);
            root.TrackSerializedObjectValue(serializedObject, _ => RefreshWarnings(warnings));

            return root;
        }

        private void RefreshWarnings(VisualElement warnings)
        {
            warnings.Clear();

            var preset = (IconCreatorPreset)target;
            int capacity = preset.divideCount * preset.divideCount;
            int assignedCount = 0;
            int missingCount = 0;

            foreach (Sprite sprite in preset.defaultIconSources)
            {
                if (!sprite)
                {
                    missingCount++;
                    continue;
                }

                assignedCount++;

                if (!sprite.texture)
                {
                    warnings.Add(new HelpBox(
                        $"'{sprite.name}' 스프라이트의 원본 텍스처를 읽을 수 없습니다.",
                        HelpBoxMessageType.Error));
                }
            }

            if (assignedCount > capacity)
            {
                warnings.Add(new HelpBox(
                    $"기본 아이콘 수({assignedCount})가 Divide Count 격자 용량({capacity})을 초과합니다.",
                    HelpBoxMessageType.Warning));
            }

            if (missingCount > 0)
            {
                warnings.Add(new HelpBox(
                    $"비어 있는(Missing) 스프라이트 항목이 {missingCount}개 있습니다.",
                    HelpBoxMessageType.Warning));
            }
        }
    }
}
#endif
