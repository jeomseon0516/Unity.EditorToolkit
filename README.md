# Jeomseon Unity Editor Toolkit

범용 Inspector Attribute의 Drawer, EditorWindow 및 ScriptableObject 제작 도구를 제공합니다.

## 설치

OpenUPM 등록 전에는 Package Manager의 **Add package from git URL**에서 다음 주소를 사용합니다.

```text
https://github.com/jeomseon0516/Unity.EditorToolkit.git#v0.2.0
```

## 리팩토링 방침

Unity가 제공하는 동등 기능과 비교해 대체 가능한 코드는 소스의 한글 TODO 주석과 CHANGELOG의 Unreleased 항목에서 추적합니다.

Attribute 선언은 `com.jeomseon.unity.attributes`에서 제공하며, Localization 전용 Attribute와 Drawer는 `com.jeomseon.unity.localization`에서 제공합니다.
