# Jeomseon Unity Editor Toolkit

범용 Inspector Attribute의 Drawer, EditorWindow 및 ScriptableObject 제작 도구를 제공합니다.

## 설치

OpenUPM 등록 전에는 Package Manager의 **Add package from git URL**에서 다음 주소를 사용합니다.

```text
https://github.com/jeomseon0516/Unity.EditorToolkit.git#v0.2.1
```

`OnChangedValueForMethodAttribute`는 특정 CustomEditor를 대체하지 않고 전역 Undo 변경 알림을
사용합니다. 다른 CustomEditor에서도 `SerializedObject`/`SerializedProperty` 또는
`Undo.RecordObject`를 통해 변경을 기록하면 콜백이 실행됩니다. Undo를 기록하지 않고 필드에 직접
대입한 변경은 Unity 공식 API로 안정적으로 관찰할 수 없으므로 지원하지 않습니다.

## 리팩토링 방침

Unity가 제공하는 동등 기능과 비교해 대체 가능한 코드는 소스의 한글 TODO 주석과 CHANGELOG의 Unreleased 항목에서 추적합니다.

Attribute 선언은 `com.jeomseon.unity.attributes`에서 제공하며, Localization 전용 Attribute와 Drawer는 `com.jeomseon.unity.localization`에서 제공합니다.
