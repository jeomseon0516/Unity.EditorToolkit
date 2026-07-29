# 변경 기록

## [0.2.1] - 2026-07-29

- Inspector 버튼을 헤더가 아닌 기본 인스펙터 본문 하단에 렌더링하도록 fallback CustomEditor 방식으로 변경했습니다.
- 자체 CustomEditor에서 동일한 위치 정책을 적용할 수 있도록 `InspectorButtonGUI.Draw` 진입점을 공개했습니다.
- `OnChangedValueForMethodAttribute` 처리를 인스펙터 Repaint 폴링에서 `Undo.postprocessModifications` 기반으로 변경했습니다.

## [0.2.0] - 2026-07-29

- 범용 Attribute 선언을 Jeomseon Unity Attributes 패키지로 분리했습니다.
- LocalizedString Attribute와 Drawer를 Localization 패키지로 이동했습니다.
- Unity 내부 InspectorWindow 리플렉션을 제거했습니다.

## [Unreleased]

- TODO(api): UI Toolkit의 PropertyDrawer·InspectorElement·SettingsProvider로 대체 가능한 IMGUI 편집기 기능을 단계적으로 이전합니다.
- 정적 이벤트와 전역 인스턴스의 Domain Reload 비활성화 호환성을 검토합니다.

## [0.1.0] - 2026-07-29

- JeomseonScriptPack의 관련 모듈을 독립 UPM 패키지로 분리했습니다.
