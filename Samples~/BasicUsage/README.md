# EditorToolkit 기본 예제

`InspectorButtonSample` 씬의 `Inspector Button Sample` GameObject를 선택합니다.

- Inspector 본문 하단에 `Injection 버튼 실행` 버튼이 한 번만 표시되는지 확인합니다.
- 버튼을 누르면 Button Click Count가 증가하고 Console에 클릭 횟수가 출력되는지 확인합니다.
- 여러 GameObject에 `InspectorInjectionSample`을 추가하고 동시에 선택했을 때 버튼이 모든 대상에 호출되는지 확인합니다.
- 버튼 실행 후 Undo/Redo가 정상적으로 동작하는지 확인합니다.
- Message를 변경했을 때 Change Count가 증가하는지 확인합니다.

씬 없이 검증하려면 임의의 GameObject에 `InspectorInjectionSample`을 직접 추가해도 됩니다.
