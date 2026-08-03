# EditorToolkit 로드맵

우선순위: `P0` 결함·안전성 → `P1` 핵심 구조 → `P2` API·성능 → `P3` 장기 확장

## 작업 순서

1. **P0-01 — 값 변경 콜백 계약 보강**
   - Undo를 기록하지 않는 CustomEditor와 직접 필드 대입에서도 누락·중복 호출을 방지합니다.
2. **P0-02 — Inspector Injection 수명 안정화**
   - Assembly Reload와 Domain Reload 설정별 콜백 중복 및 백엔드 누수를 테스트합니다.
3. **P1-01 — 내부 Inspector 접근 격리**
   - 버전별 백엔드만 내부 API를 사용하고 실패 시 Injection 기능만 비활성화합니다.
4. **P1-02 — Unity 버전 테스트 매트릭스**
   - 최소 지원 버전인 Unity 6000.3.15f1 이상의 자동 검증 환경을 구성합니다.
5. **P2-01 — IMGUI 기능의 UI Toolkit 이전**
   - PropertyDrawer, InspectorElement, SettingsProvider로 대체 가능한 순서대로 이전합니다.
6. **P2-02 — IconCreator 정확성**
   - 픽셀 경계, 알파 합성, 크기와 색 공간별 결과를 테스트합니다.
7. **P2-03 — IconCreator preset**
   - atlas 크기, 분할, importer와 TMP 생성 설정을 재사용 가능한 preset으로 제공합니다.
8. **P3-01 — Editor 기능 하위 패키지화**
   - Inspector, Window, ScriptableObject 도구를 선택 설치할 가치가 있는지 사용량으로 판단합니다.
