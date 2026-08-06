# Jeomseon Unity Editor Toolkit

## UI Toolkit 이전 현황

P2-01 작업의 일부로 `UIAnchorSetter`와 `ObjectNamingChanger`를 UI Toolkit의
`CreateGUI()` 기반으로 이전했습니다. 기존 `IMGUIHelper`는 호환성을 위해 유지하며,
`BulkComponentRemoverWindow`, `LoadableScriptableObjectDrawer` 및 SceneView 기반 UI는
후속 이전 대상입니다.

## SerializedProperty 유틸리티

`SerializedPropertyExtensions.GetPropertyValue()`는 대부분의 프로퍼티를
`SerializedProperty.boxedValue` 기반으로 반환합니다. `LayerMask`, `AnimationCurve`,
`Gradient` 및 배열 메타데이터처럼 `boxedValue`에 제약이 있는 타입은 전용 accessor를
사용합니다.

enum 프로퍼티는 `SerializedEnumData`로 반환되며 `EnumType`, `Value`, `Index`, `Name`,
`DisplayName` 정보를 제공합니다. enum 정보가 필요한 경우
`SerializedPropertyExtensions.GetEnumData()`를 직접 사용할 수도 있습니다.

Attribute와 관계없는 공통 Editor API, EditorWindow 및 ScriptableObject 제작 도구를 제공합니다.

## 설치

OpenUPM 등록 전에는 Package Manager의 **Add package from git URL**에서 다음 주소를 사용합니다.

```text
https://github.com/jeomseon0516/Unity.EditorToolkit.git#v0.3.1
```

## 리팩토링 방침

Unity가 제공하는 동등 기능과 비교해 대체 가능한 코드는 소스의 한글 TODO 주석과 CHANGELOG의 Unreleased 항목에서 추적합니다.

범용 Attribute의 선언과 Editor 구현은 `com.jeomseon.unity.attributes`가 소유합니다.
기능 전용 Attribute와 Drawer는 해당 기능 패키지가 소유합니다.
