# Jeomseon Unity Editor Toolkit

범용 Inspector Attribute의 Drawer, EditorWindow 및 ScriptableObject 제작 도구를 제공합니다.

## 설치

OpenUPM 등록 전에는 Package Manager의 **Add package from git URL**에서 다음 주소를 사용합니다.

```text
https://github.com/jeomseon0516/Unity.EditorToolkit.git#v0.3.1
```

## Inspector 확장 방식

`InspectorButtonAttribute`는 기존 CustomEditor가 있어도 본문 하단에 버튼을 추가할 수 있도록
Inspector Injection을 사용합니다. 공통 생명주기와 Drawer 체인은 인터페이스로 관리하고,
Unity 6000.3.15f1 이상을 지원하며 Unity 6 Inspector 백엔드만 사용합니다.

이 기능은 Unity가 공개하지 않은 InspectorWindow/InspectorElement 내부 구조를 제한적으로
조회하므로 Unity 패치 버전에 따라 동작하지 않을 수 있습니다. 필요한 내부 멤버를 찾지 못하면
에디터 전체에 영향을 주지 않고 Injection만 중단하며 경고를 출력합니다. 자체 CustomEditor에서는
`InspectorButtonGUI.Draw(this)`를 직접 호출하는 안정적인 대체 경로도 사용할 수 있습니다.

`OnChangedValueForMethodAttribute`는 특정 CustomEditor를 대체하지 않고 전역 Undo 변경 알림을
사용합니다. 다른 CustomEditor에서도 `SerializedObject`/`SerializedProperty` 또는
`Undo.RecordObject`를 통해 변경을 기록하면 콜백이 실행됩니다. Undo를 기록하지 않고 필드에 직접
대입한 변경은 Unity 공식 API로 안정적으로 관찰할 수 없으므로 지원하지 않습니다.

## 리팩토링 방침

Unity가 제공하는 동등 기능과 비교해 대체 가능한 코드는 소스의 한글 TODO 주석과 CHANGELOG의 Unreleased 항목에서 추적합니다.

Attribute 선언은 `com.jeomseon.unity.attributes`에서 제공하며, Localization 전용 Attribute와 Drawer는 `com.jeomseon.unity.localization`에서 제공합니다.
