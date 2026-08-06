# 변경 기록

## [Unreleased]

- `UIAnchorSetter`와 `ObjectNamingChanger`를 UI Toolkit `CreateGUI()` 기반으로 이전했습니다.
  기존 IMGUI 동작과 메뉴 진입점은 유지했습니다.

- `SerializedPropertyExtensions.GetPropertyValue()`를 `boxedValue` 기반으로 정리하고,
  enum의 실제 값과 선언 이름 및 표시 이름을 함께 제공하는 public `SerializedEnumData`
  유틸리티를 추가했습니다.

- 범용 Attribute의 Drawer, Inspector 주입, 메서드 실행 구현과 샘플을 Attributes 패키지로 이동했습니다.
- Runtime 및 Editor asmdef와 package.json에서 Attributes 의존성을 제거했습니다.

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

## [0.1.0] - 2026-07-29

- JeomseonScriptPack의 관련 모듈을 독립 UPM 패키지로 분리했습니다.


## [0.3.3] - 2026-08-05

- Unity 6000.5.7f1을 최소 지원 버전으로 상향했습니다.
