namespace Jeomseon.PackageNotes
{
    internal static class PackageArchitectureNotes
    {
        // TODO(api): UI Toolkit의 PropertyDrawer·InspectorElement·SettingsProvider로 대체 가능한 IMGUI 편집기 기능을 단계적으로 이전합니다.
        // TODO(editor-injection): Method Attribute의 임의 CustomEditor 공존을 위해 내부 Inspector 구조를
        // 사용하는 기능은 버전별 백엔드로 격리하고, 실패 시 해당 기능만 비활성화합니다.
        // 가능한 기능은 PropertyDrawer, Undo.postprocessModifications 등 공식 확장 지점을 우선 사용합니다.
        // TODO(test-matrix): Unity 2022.3 LTS와 지원하는 Unity 6 패치 버전별로 Inspector Injection을 검증합니다.
        // TODO(lifecycle): 정적 이벤트와 전역 인스턴스는 Domain Reload 비활성화 환경에서 초기화 상태가 남는지 검증합니다.
    }
}
