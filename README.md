# Jeomseon Unity Editor Toolkit

한국어 | [English](README.en.md)

패키지 간 공통 Editor API와 독립 제작 도구를 제공합니다. IconCreator는 개별 Sprite를 격자 텍스처와 TMP SpriteAsset으로 변환합니다.

## 빠르게 시작하기

Jeomseon/Icon Creator 열기 → 원본 Sprite·크기·Divide Count 설정 또는 Preset 로드 → Preview → Create Icon. 다른 도구는 Jeomseon/Tool 메뉴에서 선택합니다.

1. `Jeomseon/Icon Creator`를 엽니다.
2. Import한 샘플 Preset을 로드하거나 원본 Sprite, Size, Divide Count를 지정합니다.
3. Preview로 격자를 확인하고 Create Icon으로 저장합니다. TMP 출력은 Preset에서 선택합니다.

## 계약과 주의사항

SpriteAtlas의 런타임 패킹과 목적이 다릅니다. Editor API는 Editor 어셈블리에서만 참조합니다. Preview 리소스는 도구가 관리하며 입력 변경 시 다시 Preview해야 합니다.

## 요구 사항

- Unity `6000.6.0f1`
- 패키지: `com.jeomseon.unity.editor-toolkit` `0.7.3`
- `com.jeomseon.unity.core`: `0.3.2`
- `com.unity.2d.sprite`: `1.0.0`
- `com.unity.ugui`: `2.0.0`

## 설치

프로젝트 `Packages/manifest.json`의 기존 dependencies와 registry를 유지하면서 다음 항목을 병합합니다.

```json
{
  "scopedRegistries": [
    {
      "name": "OpenUPM",
      "url": "https://package.openupm.com",
      "scopes": [
        "com.jeomseon"
      ]
    }
  ],
  "dependencies": {
    "com.jeomseon.unity.editor-toolkit": "0.7.3"
  }
}
```

또는 Package Manager의 **Install package from git URL**을 사용합니다:

```text
https://github.com/jeomseon0516/Unity.EditorToolkit.git#v0.7.3
```

## 설계와 동작

### 패키지 경계

반복되는 EditorWindow, SerializedProperty 접근, 타입 탐색을 공용 API로 제공합니다.
범용 Inspector Attribute와 Drawer는 Attributes 패키지가 소유합니다. 게임 기능 패키지는
필요한 Editor API만 단방향으로 참조하며 런타임 어셈블리에서 UnityEditor API를 사용하지 않습니다.

### IconCreator의 입력·출력

1. `Jeomseon/Icon Creator`를 엽니다.
2. Preset을 선택해 Load하거나 Size, Divide Count, 원본 Sprite를 설정합니다.
3. Preview에서 실제 배치를 확인합니다. 입력 변경은 이전 Preview를 무효화합니다.
4. Create Icon으로 프로젝트 안에 PNG와 Sprite 메타데이터를 저장합니다.
5. Preset의 GenerateTmpSpriteAsset이 켜져 있으면 TMP SpriteAsset도 생성합니다.

Size는 셀 크기, Divide Count는 한 줄의 최대 셀 수입니다. 실제 출력은 입력 수에 따라 더 작을 수
있으므로 항상 Preview를 확인합니다. null Sprite와 용량 초과 입력은 정상 생성 경로가 아닙니다.
Preview Texture는 창이 소유하며 재생성·창 종료 때 해제합니다. 저장된 에셋은 프로젝트가 소유합니다.

SpriteAtlas는 런타임/빌드 패킹 최적화이고 이 도구는 고정 격자 제작과 TMP 메타데이터 생성입니다.
같은 작업이라고 가정하면 안 됩니다. 실제 예제는
[샘플 안내](Samples~/BasicUsage/README.md)의 Preset/4색 Sprite로 확인합니다.

### 공통 API와 오용 방지

| 영역 | API | 주의점 |
| --- | --- | --- |
| 직렬화 값 | `SerializedPropertyExtensions` | Unity 직렬화 타입에 맞는 accessor 사용 |
| enum 정보 | `SerializedEnumData` / `GetEnumData` | index와 실제 enum 값을 구분 |
| 타입 탐색 | `EditorTypeDiscovery` | Editor TypeCache 기반 |
| 객체 그래프 | `SerializedObjectGraphTraversal` | Unity 참조와 managed 참조의 경계를 구분 |
| UI 구성 | `EditorDropdownController<T>`, `EditorGUILayoutActions` | 사용자 창이 callback과 임시 리소스 수명 관리 |
| ScriptableObject | `ScriptableObjectHandler` | 저장 경로·Undo·프로젝트 에셋 변경을 확인 |

[IconCreator](Editor/Tool/IconCreator.cs) → [Preset](Editor/Tool/IconCreatorPreset.cs) →
[SerializedProperty helpers](Editor/Extensions/SerializedPropertyExtensions.cs) 순서로 읽습니다.
UI Anchor Setter, Object Naming Changer, Bulk Component Remover는 실제 프로젝트 객체를 바꾸므로
선택 범위와 Undo를 확인하며 사용합니다. 기본 API 동작은 샘플 EditorWindow에서 비교할 수 있습니다.

## API 사용과 상세 설명

### Icon Creator

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

### SerializedProperty 유틸리티

`SerializedPropertyExtensions.GetPropertyValue()`는 대부분의 프로퍼티를
`SerializedProperty.boxedValue` 기반으로 반환합니다. `LayerMask`, `AnimationCurve`,
`Gradient` 및 배열 메타데이터처럼 `boxedValue`에 제약이 있는 타입은 전용 accessor를
사용합니다.

enum 프로퍼티는 `SerializedEnumData`로 반환되며 `EnumType`, `Value`, `Index`, `Name`,
`DisplayName` 정보를 제공합니다. enum 정보가 필요한 경우
`SerializedPropertyExtensions.GetEnumData()`를 직접 사용할 수도 있습니다.

Attribute와 관계없는 공통 Editor API, EditorWindow 및 ScriptableObject 제작 도구를 제공합니다.

## 샘플 실행

**Package Manager > Samples**에서 예제를 Import합니다. 포함된 Scene 또는 Editor 도구를 열고 필요한 Setup 메뉴를 먼저 실행합니다.

- [Basic Usage](Samples~/BasicUsage/README.md)

## 추가 문서

[전체 코드 지도와 패키지 구성](Documentation~/Overview.md)
- [Changelog](CHANGELOG.md)

## 라이선스

[License](LICENSE.md)
