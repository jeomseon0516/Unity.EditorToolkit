namespace Jeomseon.PackageNotes
{
    internal static class PackageArchitectureNotes
    {
        // TODO(api): UI Toolkit의 PropertyDrawer·InspectorElement·SettingsProvider로 대체 가능한 IMGUI 편집기 기능을 단계적으로 이전합니다.
        // TODO(editor): Unity 내부 InspectorWindow 구현을 리플렉션하지 않고 Editor.finishedDefaultHeaderGUI,
        // CustomEditor, PropertyDrawer 등 공식 확장 지점만 사용합니다.
        // TODO(lifecycle): 정적 이벤트와 전역 인스턴스는 Domain Reload 비활성화 환경에서 초기화 상태가 남는지 검증합니다.
    }
}
