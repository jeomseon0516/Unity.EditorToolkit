# Jeomseon Unity Editor Toolkit

[English](Overview.en.md) · [설치](../README.md)

## 목적

패키지 간 공통 Editor API와 독립 제작 도구를 제공합니다. IconCreator는 개별 Sprite를 격자 텍스처와 TMP SpriteAsset으로 변환합니다.

## 사용 흐름

Jeomseon/Icon Creator 열기 → 원본 Sprite·크기·Divide Count 설정 또는 Preset 로드 → Preview → Create Icon. 다른 도구는 Jeomseon/Tool 메뉴에서 선택합니다.

## 계약과 제한

SpriteAtlas의 런타임 패킹과 목적이 다릅니다. Editor API는 Editor 어셈블리에서만 참조합니다. Preview 리소스는 도구가 관리하며 입력 변경 시 다시 Preview해야 합니다.

## 패키지 요구 사항

- Package ID: `com.jeomseon.unity.editor-toolkit`
- Version: `0.7.3`
- Unity: `6000.6` / `0f1`
- `com.jeomseon.unity.core`: `0.3.2`
- `com.unity.2d.sprite`: `1.0.0`
- `com.unity.ugui`: `2.0.0`

## 샘플과 검증

Package Manager > Samples에서 다음 예제를 Import하세요. 포함된 Scene 또는 Editor 도구를 열고 샘플 README의 Setup 절차를 먼저 수행합니다.

- [Basic Usage](../Samples~/BasicUsage/README.md)

## 코드 지도

- [AnimationCurveReference](../Runtime/ScriptableObjects/AnimationCurveReference.cs)
- [LoadableScriptableObject](../Runtime/ScriptableObjects/LoadableScriptableObject.cs)
- [NumberReference](../Runtime/ScriptableObjects/NumberReference.cs)
- [SpriteReference](../Runtime/ScriptableObjects/SpriteReference.cs)
- [BulkComponentRemoverWindow](../Editor/BulkComponentRemoverWindow.cs)
- [EditorHelper](../Editor/EditorHelper.cs)
- [EditorTypeDiscovery](../Editor/EditorTypeDiscovery.cs)
- [SerializedEnumData](../Editor/Extensions/SerializedEnumData.cs)
- [SerializedObjectExtensions](../Editor/Extensions/SerializedObjectExtensions.cs)
- [SerializedPropertyExtensions](../Editor/Extensions/SerializedPropertyExtensions.cs)
- [EditorDropdownController](../Editor/GUI/EditorDropdown.cs)
- [EditorGUILayoutActions](../Editor/GUI/EditorGUILayoutActions.cs)
- [GUIStyleTexture](../Editor/GUI/GUIStyleTexture.cs)
- [ToggleEnumerator](../Editor/GUI/ToggleEnumerator.cs)
- [ScriptableObjectHandler](../Editor/ScriptableObjects/Editor/ScriptableObjectHandler.cs)
- [SerializedObjectGraphTraversal](../Editor/SerializedObjectGraphTraversal.cs)
- [SerializedPropertyReflection](../Editor/SerializedPropertyReflection.cs)
- [IconCreatorPreset](../Editor/Tool/IconCreatorPreset.cs)
- [ObjectNamingChanger](../Editor/Window/ObjectNamingChanger.cs)
- [UIAnchorSetter](../Editor/Window/UIAnchorSetter.cs)

[상세 설계](Design.md)
