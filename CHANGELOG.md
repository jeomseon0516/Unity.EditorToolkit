# 변경 기록

## [0.3.2] - 2026-07-29

- Runtime·Editor·Samples 어셈블리의 `rootNamespace`와 소스 파일 위치를 namespace에 맞게 정리했습니다.

## [0.3.1] - 2026-07-29

- Inspector Injection과 값 변경 콜백을 확인하는 `Basic Usage` 샘플을 추가했습니다.

## [0.3.0] - 2026-07-29

- Inspector Injection 공통 계약, 서비스, Drawer 체인을 추가했습니다.
- Unity 2022.3~2023 계열과 Unity 6의 내부 Inspector 구현을 별도 백엔드로 분리했습니다.
- `InspectorButtonAttribute`가 기존 CustomEditor와 함께 본문 하단에 렌더링되도록 Injection Drawer로 변경했습니다.
- 내부 구조 탐색 실패가 EditorToolkit 전체에 전파되지 않도록 방어 처리와 진단 경고를 추가했습니다.
- `InspectorButtonGUI.Draw`를 명시적이고 안정적인 대체 경로로 유지했습니다.

## [0.2.1] - 2026-07-29

- Inspector 버튼을 헤더가 아닌 기본 인스펙터 본문 하단에 렌더링하도록 fallback CustomEditor 방식으로 변경했습니다.
- 자체 CustomEditor에서 동일한 위치 정책을 적용할 수 있도록 `InspectorButtonGUI.Draw` 진입점을 공개했습니다.
- `OnChangedValueForMethodAttribute` 처리를 인스펙터 Repaint 폴링에서 `Undo.postprocessModifications` 기반으로 변경했습니다.

## [0.2.0] - 2026-07-29

- 범용 Attribute 선언을 Jeomseon Unity Attributes 패키지로 분리했습니다.
- LocalizedString Attribute와 Drawer를 Localization 패키지로 이동했습니다.
- Unity 내부 InspectorWindow 리플렉션을 제거했습니다.

## [Unreleased]

- `StringBuilderPool` 이동에 맞춰 `Jeomseon.Text`를 사용하고 GameObject Pooling 패키지 의존성을 제거했습니다.
- `OnChangedValueForMethodAttribute` 메서드 탐색을 Unity `TypeCache` 기반 전용 캐시로 변경하고, 상속된 private 메서드와 override 중복을 일관되게 처리합니다.
- `InspectorButtonAttribute`와 `OnChangedValueForMethodAttribute`가 TypeCache 탐색, 상속 처리, 매개변수 메타데이터 및 안전한 메서드 호출 경로를 공유하도록 공통화했습니다.
- 공통 메서드 메타데이터와 캐시를 `EditorMethodTriggerAttribute` 기반으로 제한하고, 기능별 Attribute를 Trigger 메타데이터로 처리하도록 변경했습니다.
- Trigger 메서드 명칭을 `EditorTriggeredMethod`로 통일하고, 공통 매개변수 메타데이터·검증기·호출 요청 객체 및 Trigger Handler Registry를 추가했습니다.
- TODO(api): UI Toolkit의 PropertyDrawer·InspectorElement·SettingsProvider로 대체 가능한 IMGUI 편집기 기능을 단계적으로 이전합니다.
- 정적 이벤트와 전역 인스턴스의 Domain Reload 비활성화 호환성을 검토합니다.

## [0.1.0] - 2026-07-29

- JeomseonScriptPack의 관련 모듈을 독립 UPM 패키지로 분리했습니다.
