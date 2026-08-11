# EditorToolkit 로드맵

우선순위: `P0` 결함·안전성 → `P1` 핵심 구조 → `P2` API·성능 → `P3` 장기 확장

## 작업 순서

1. **P1-01 — Unity 버전 테스트 매트릭스 (완료)**
   - 최소 지원 버전인 Unity 6000.5.7f1 이상의 자동 검증 환경을 구성합니다.
2. **P2-01 — IMGUI 기능의 UI Toolkit 이전 (완료)**
   - `UIAnchorSetter`, `ObjectNamingChanger`, `BulkComponentRemoverWindow`,
     `LoadableScriptableObjectDrawer`, `IconCreator`(CreateGUI + ListView 기반 Icon Sources)
     이전 완료.
   - `SceneViewInnerWindow`는 제거하고 소비 패키지인 `Jeomseon.Unity.GridTileSystem`을
     Unity `Overlay` API 기반 `HexTileOptionOverlay`로 이전했습니다.
3. **P2-02 — IconCreator 정확성 (완료)**
   - `sprite.textureRect` 기반 픽셀 추출로 스프라이트 경계 무시 결함을 수정하고
     Preview 임시 텍스처 누수를 해제했습니다.
4. **P2-03 — IconCreator preset (완료)**
   - `IconCreatorPreset`(`ScriptableObject`)으로 atlas 크기, Divide Count, importer,
     TMP 생성 설정, 기본 아이콘 목록을 저장/재사용합니다.
5. **P3-01 — Editor 기능 하위 패키지화 (보류)**
   - Inspector, Window, ScriptableObject 도구를 선택 설치할 가치가 있는지 사용량으로 판단합니다.
   - 현재는 판단할 사용량 근거가 없어 분리하지 않습니다. 실제 소비 프로젝트가 늘어나
     특정 도구 그룹만 필요하다는 구체적 근거가 생기면 재검토합니다.
6. **P3-02 — Editor Locale 공용화 (보류)**
   - `Jeomseon.Unity.Localization`이 자체 Editor 어셈블리에 `EditorLocaleText`(OS 언어 기반
     ko/en 분기 + `EditorPrefs` override) · `EditorLocaleOverride` · `EditorLocaleSettingsProvider`
     (Preferences에서 언어 override)를 처음 도입했습니다. 이 기능은 Localization 고유 개념이
     아니라 Editor 경고·버튼 텍스트를 다국어로 보여주고 싶은 임의의 패키지에 공통으로 필요한
     Editor 인프라입니다.
   - 두 번째 소비 패키지(Editor 기능이 있고 동일하게 ko/en 분기가 필요한 패키지)가 실제로
     생기면, 이 타입들을 `EditorToolkit`으로 옮기고 각 소비 패키지가 `EditorToolkit`을
     의존성으로 가져와 사용하는 구조로 전환합니다(단방향 의존은 `ADR-0006`으로 이미 허용됨,
     별도 어댑터 불필요).
   - 현재는 소비 사례가 `Jeomseon.Unity.Localization` 하나뿐이라 이전하지 않습니다. 근거 없는
     사전 이동은 API 이동 비용만 발생시킵니다(`ADR-0004`와 동일한 논리).
