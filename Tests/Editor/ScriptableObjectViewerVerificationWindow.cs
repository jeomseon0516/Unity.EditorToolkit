using Jeomseon.Unity.EditorToolkit.Editor.Window;
using UnityEditor;

namespace Jeomseon.Unity.EditorToolkit.Tests
{
    // internal ScriptableObjectViewer의 디자인·동작을 UI Toolkit 전환 후 육안으로 확인하기 위한 검증 전용 창.
    // 공개 API나 패키지 기능이 아니며, 패키지를 testables에 둔 프로젝트에서만 컴파일된다.
    internal sealed class ScriptableObjectViewerVerificationWindow : ScriptableObjectViewer
    {
        internal const string MenuPath = "Jeomseon/Tests/ScriptableObject Viewer";

        [MenuItem(MenuPath)]
        private static void ShowWindow()
        {
            GetWindow<ScriptableObjectViewerVerificationWindow>("ScriptableObject Viewer");
        }

        protected override ScrollBuildOption[] GetTargetScriptableObjectType() => new[]
        {
            new ScrollBuildOption(typeof(ViewerVerificationItemData), "Items"),
            new ScrollBuildOption(typeof(ViewerVerificationSkillData), "Skills")
        };
    }
}
