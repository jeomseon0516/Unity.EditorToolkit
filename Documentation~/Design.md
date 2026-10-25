# EditorToolkit: 제작 도구와 공통 API

[English](Design.en.md) · [설치](../README.md)

## 패키지 경계

반복되는 EditorWindow, SerializedProperty 접근, 타입 탐색을 공용 API로 제공합니다.
범용 Inspector Attribute와 Drawer는 Attributes 패키지가 소유합니다. 게임 기능 패키지는
필요한 Editor API만 단방향으로 참조하며 런타임 어셈블리에서 UnityEditor API를 사용하지 않습니다.

## IconCreator의 입력·출력

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
[샘플 안내](../Samples~/BasicUsage/README.md)의 Preset/4색 Sprite로 확인합니다.

## 공통 API와 오용 방지

| 영역 | API | 주의점 |
| --- | --- | --- |
| 직렬화 값 | `SerializedPropertyExtensions` | Unity 직렬화 타입에 맞는 accessor 사용 |
| enum 정보 | `SerializedEnumData` / `GetEnumData` | index와 실제 enum 값을 구분 |
| 타입 탐색 | `EditorTypeDiscovery` | Editor TypeCache 기반 |
| 객체 그래프 | `SerializedObjectGraphTraversal` | Unity 참조와 managed 참조의 경계를 구분 |
| UI 구성 | `EditorDropdownController<T>`, `EditorGUILayoutActions` | 사용자 창이 callback과 임시 리소스 수명 관리 |
| ScriptableObject | `ScriptableObjectHandler` | 저장 경로·Undo·프로젝트 에셋 변경을 확인 |

[IconCreator](../Editor/Tool/IconCreator.cs) → [Preset](../Editor/Tool/IconCreatorPreset.cs) →
[SerializedProperty helpers](../Editor/Extensions/SerializedPropertyExtensions.cs) 순서로 읽습니다.
UI Anchor Setter, Object Naming Changer, Bulk Component Remover는 실제 프로젝트 객체를 바꾸므로
선택 범위와 Undo를 확인하며 사용합니다. 기본 API 동작은 샘플 EditorWindow에서 비교할 수 있습니다.
