# 변경 기록

## [0.7.1] - 2026-09-03

- Unity 최소 버전을 `6000.5.7f1` → `6000.6.0f1`로 상향했습니다. 코드·API 변경은 없습니다.

## [0.7.0] - 2026-08-13

- **(Breaking)** 네임스페이스를 `Jeomseon.Unity.EditorToolkit`(Runtime)/`Jeomseon.Unity.EditorToolkit.Editor`(Editor)
  기준으로 정리했습니다(`Jeomseon.ScriptableObjects`→`Jeomseon.Unity.EditorToolkit.ScriptableObjects`,
  `Jeomseon.Editor`→`Jeomseon.Unity.EditorToolkit.Editor`, 하위 `.Extensions`/`.GUI`/`.Tool`/`.Window`도
  동일). `Editor/Editor/` 중복 폴더를 `Editor/`로 평탄화했습니다(GUID 보존). 워크스페이스 전체
  네임스페이스 규칙(`AGENTS.md` 참고)을 적용한 것입니다.

## [0.6.0] - 2026-08-11

- 워크스페이스 명명 규칙에 맞춰 Unity가 직렬화하는 필드(`[SerializeField] private` 및
  `IconCreatorPreset`의 public 필드)를 정리하고 기존 이름을 `[FormerlySerializedAs]`로
  보존했습니다. `LoadableScriptableObjectDrawer`의 `FindPropertyRelative` 문자열과
  `IconCreator`/`IconCreatorPresetEditor`의 필드 접근부도 함께 갱신했습니다. 기존
  Scene·Prefab·Preset 자산의 직렬화된 값은 그대로 유지됩니다.
- **(Breaking)** `IconCreatorPreset`의 public 필드 이름을 PascalCase로 정리했습니다:
  `size`→`Size`, `divideCount`→`DivideCount`, `textureType`→`TextureType`,
  `spriteImportMode`→`SpriteImportMode`, `generateTmpSpriteAsset`→`GenerateTmpSpriteAsset`,
  `defaultIconSources`→`DefaultIconSources`. 코드에서 이 필드를 직접 참조하던 외부 소비처가
  있다면 이름을 갱신해야 합니다.

## [0.5.0] - 2026-08-10

- **(Breaking)** GridTileSystem에만 사용되던 `SceneViewInnerWindow`를 제거했습니다. 소비 패키지는
  Unity `Overlay` API로 직접 구현하는 책임을 갖습니다(`Jeomseon.Unity.GridTileSystem`은
  `HexTileOptionOverlay`로 이전 완료).
- `IconCreator`를 IMGUI `OnGUI()`에서 UI Toolkit `CreateGUI()`로 이전했습니다. Icon Sources는
  `ReorderableList`(IMGUI) 대신 `ListView`(reorderable, add/remove footer)로 교체했고,
  용량 초과 시 추가를 막던 기존 제약은 `itemsAdded` 콜백으로 재구현했습니다. 픽셀 연산·저장
  로직(`CreateAndSaveAtlas`/`BuildSpriteRects`/`GenerateTmpSpriteAsset`/`BuildPreviewAtlas`)은
  변경하지 않았습니다. "Create Icon" 버튼은 미리보기 텍스처가 없으면 비활성화되도록
  개선했습니다(기존은 클릭해도 조용히 아무 동작을 하지 않았습니다).
- `IconCreator` 입력 변경과 창 종료 시 Preview 텍스처를 즉시 해제하고, 유효한 Sprite가 없으면
  Preview 생성을 비활성화하도록 수명·입력 검증을 보강했습니다.

## [0.4.0] - 2026-08-10

- **(Breaking)** 책임 불명확 grab-bag이던 `IMGUIHelper`(구 `EditorGUIHelper`)를 제거했습니다.
  실사용 중이던 `ActionEditorVerticalBox`/`ActionEditorVertical`은 `Jeomseon.Editor.GUI.EditorGUILayoutActions`로,
  `GetTexture2D`는 `Jeomseon.Editor.GUI.GUIStyleTexture`로 이름과 책임을 분리해 이전했습니다.
  `ActionEditorVerticalBox`의 `VerticalScope` 즉시 Dispose 버그도 함께 수정했습니다. 이 개명·이동으로
  깨져 있던 `Jeomseon.Unity.UI`의 크로스 패키지 참조도 함께 복구했습니다.
- `ScriptableObjectScrollView`의 선택 강조색이 Unity 6000.5에서 obsolete된
  `ITextSelection.selectionColor`를 사용하던 것을 USS 커스텀 프로퍼티(`--unity-selection-color`)
  기반 `customStyle.TryGetValue`로 교체했습니다. `#3E3E3E` 하드코딩 3중 중복도 상수로 통합했습니다.
- `ToggleEnumerator<T>.SelectEnumeratedToggles`가 데이터 소스 미설정 시 `NullReferenceException`을
  던지던 결함을 방어 처리했습니다.
- `IconCreator.OnGUI()`(220줄)를 `CreateAndSaveAtlas`/`BuildSpriteRects`/`GenerateTmpSpriteAsset`/
  `BuildPreviewAtlas`로 나눴습니다(동작 변경 없음). `IconCreator`가 Unity Sprite Atlas 시스템과
  다른 목적(TMP 인라인 스프라이트용 고정 격자 아틀라스)임을 클래스 문서와 README에 명시했습니다.
- `BulkComponentRemoverWindow`의 반복되던 Toggle 생성·바인딩 코드를 `AddBoundToggle` 헬퍼로 통합했습니다.
- enum 값, `protected`/`public` 필드, 로컬 함수·메서드 명명을 C# 관례(PascalCase)에 맞게 정리했습니다.
- `Samples~/BasicUsage`를 추가했습니다: `EditorToolkitSample`(EditorWindow)이 `EditorGUILayoutActions`,
  `GUIStyleTexture`, `ToggleEnumerator<T>`, `EditorDropdownController<T>`, `EditorTypeDiscovery`,
  `SerializedPropertyExtensions.GetPropertyType()`를 시연·검증합니다. `IconCreator` 검증용으로
  128x128 샘플 스프라이트 4개와 이를 참조하는 `SampleIconCreatorPreset`도 함께 포함했습니다.

- `IconCreatorPreset`(`ScriptableObject`, `Create > Jeomseon > Icon Creator Preset`)을 추가해
  atlas 크기·Divide Count·importer 설정(`TextureImporterType`, `SpriteImportMode`)·
  TMP_SpriteAsset 생성 여부·기본 아이콘 목록을 프로젝트별로 저장/재사용할 수 있게 했습니다.
  `IconCreator` 창에서 Preset을 불러오거나(Load) 현재 상태를 새/기존 Preset에 저장(Save As)할
  수 있고, 마지막으로 사용한 Preset은 프로젝트별 `EditorPrefs`(`PlayerSettings.productGUID` 기준)로
  기억합니다. Preset Inspector는 기본 아이콘 수가 Divide Count 격자 용량을 초과하는 경우와
  비어 있는/텍스처를 읽을 수 없는 스프라이트 항목을 경고로 표시합니다.

- `IconCreator`의 아이콘 픽셀 추출이 `sprite.texture`(packing/sheet 원본 전체)를 그대로 읽어
  스프라이트 경계를 무시하던 결함을 수정했습니다. `sprite.textureRect`로 실제 스프라이트
  영역만 잘라내도록 변경했고, Preview마다 누수되던 임시 `Texture2D`를 해제했습니다.
  `Create Icon`의 SpriteRect 메타 계산도 별도로 재계산하지 않고 실제 저장되는
  `_iconTexture` 크기를 기준으로 삼도록 정리해, 아이콘 수가 `Divide Count`보다 적을 때
  발생하던 경계 좌표 불일치를 제거했습니다.

- `LoadableScriptableObjectDrawer`를 `PropertyDrawer.OnGUI` + `ReorderableList`(IMGUI)에서
  `CreatePropertyGUI()` + UI Toolkit `ListView`(`reorderable`, foldout 헤더)로 이전했습니다.
  자산 재스캔·읽기 전용 항목 표시 동작은 유지했습니다.

- `BulkComponentRemoverWindow`를 UI Toolkit `CreateGUI()` 기반으로 이전했습니다.
  `LayerMaskField`/`TagField`로 레이어·태그 필터 UI를 대체했고, 타입 선택 팝업에
  검색 필터를 추가했습니다. 동작과 메뉴 진입점은 유지했습니다.

- 사용되지 않던 `EditorScrollController`(`Editor/Editor/GUI/EditorScrollView.cs`)를 제거했습니다.
  `ScriptableObjectViewer`의 `IMGUIContainer`는 임의의 `ScriptableObject`가 제공하는
  기본 `Editor.OnInspectorGUI()`(IMGUI 전용 계약)를 호스팅하기 위한 필수 경로이므로 유지합니다.

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
