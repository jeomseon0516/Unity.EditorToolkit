# Jeomseon Unity Editor Toolkit

## UI Toolkit 이전 현황

P2-01 작업의 일부로 `UIAnchorSetter`, `ObjectNamingChanger`, `BulkComponentRemoverWindow`,
`LoadableScriptableObjectDrawer`를 UI Toolkit(`CreateGUI()`/`CreatePropertyGUI()`) 기반으로
이전했습니다. 책임이 불명확했던 `IMGUIHelper`는 제거했고, 실사용 중이던 레이아웃 헬퍼는
`EditorGUILayoutActions`/`GUIStyleTexture`로 이름과 책임을 분리해 재구성했습니다.
`IconCreator`와 SceneView 기반 UI는 후속 이전 대상입니다.

## Icon Creator

`IconCreator`(`Jeomseon/Icon Creator`)는 여러 개별 아이콘 스프라이트를 고정 격자
텍스처로 합치고, TextMeshPro의 `TMP_SpriteAsset`(인라인 스프라이트용)까지 함께
생성합니다. Unity의 Sprite Atlas 시스템(`com.unity.2d.sprite`)은 흩어진 스프라이트를
빌드/런타임에 자동 패킹해 드로우콜을 줄이는 별개의 최적화 도구로, 고정 격자 배치나
TMP_SpriteAsset 생성을 지원하지 않습니다. TextMeshPro 자체의 Sprite Asset Creator도
이미 합쳐진 아틀라스와 스프라이트 메타데이터가 있다는 걸 전제로 동작하므로, N개의
흩어진 원본 아이콘을 격자로 합치는 단계는 Unity/TMP 어느 쪽도 자동화하지 않습니다.
`IconCreatorPreset`(`ScriptableObject`)으로 크기·Divide Count·기본 아이콘 목록을
저장/재사용할 수 있습니다. `Samples~/BasicUsage`에 검증용 샘플 스프라이트 4개와
프리셋이 포함되어 있습니다.

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
https://github.com/jeomseon0516/Unity.EditorToolkit.git#v0.4.0
```

## 리팩토링 방침

Unity가 제공하는 동등 기능과 비교해 대체 가능한 코드는 소스의 한글 TODO 주석과 CHANGELOG의 Unreleased 항목에서 추적합니다.

범용 Attribute의 선언과 Editor 구현은 `com.jeomseon.unity.attributes`가 소유합니다.
기능 전용 Attribute와 Drawer는 해당 기능 패키지가 소유합니다.
