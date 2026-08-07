# EditorToolkit 로드맵

우선순위: `P0` 결함·안전성 → `P1` 핵심 구조 → `P2` API·성능 → `P3` 장기 확장

## 작업 순서

1. **P1-01 — Unity 버전 테스트 매트릭스 (완료)**
   - 최소 지원 버전인 Unity 6000.5.7f1 이상의 자동 검증 환경을 구성합니다.
2. **P2-01 — IMGUI 기능의 UI Toolkit 이전**
   - PropertyDrawer, InspectorElement, SettingsProvider로 대체 가능한 순서대로 이전합니다.
3. **P2-02 — IconCreator 정확성**
   - 픽셀 경계, 알파 합성, 크기와 색 공간별 결과를 테스트합니다.
4. **P2-03 — IconCreator preset**
   - atlas 크기, 분할, importer와 TMP 생성 설정을 재사용 가능한 preset으로 제공합니다.
5. **P3-01 — Editor 기능 하위 패키지화 (보류)**
   - Inspector, Window, ScriptableObject 도구를 선택 설치할 가치가 있는지 사용량으로 판단합니다.
   - 현재는 판단할 사용량 근거가 없어 분리하지 않습니다. 실제 소비 프로젝트가 늘어나
     특정 도구 그룹만 필요하다는 구체적 근거가 생기면 재검토합니다.
