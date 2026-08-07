# 변경 기록

## [Unreleased]

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
