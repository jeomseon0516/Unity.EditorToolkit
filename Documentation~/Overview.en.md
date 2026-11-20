# Jeomseon Unity Editor Toolkit

[한국어](Overview.md) · [Installation](../README.en.md)

## Purpose

Provides shared Editor APIs and standalone authoring tools. IconCreator turns individual Sprites into a grid texture and TMP SpriteAsset.

## Usage flow

Open Jeomseon/Icon Creator, set source Sprites/size/Divide Count or load a Preset, Preview, then Create Icon. Other utilities live under Jeomseon/Tool.

## Contracts and limitations

This differs from runtime SpriteAtlas packing. Reference Editor APIs only from Editor assemblies. The tool owns preview resources; regenerate the preview after input changes.

## Package requirements

- Package ID: `com.jeomseon.unity.editor-toolkit`
- Version: `0.7.3`
- Unity: `6000.6` / `0f1`
- `com.jeomseon.unity.core`: `0.3.2`
- `com.unity.2d.sprite`: `1.0.0`
- `com.unity.ugui`: `2.0.0`

## Samples and verification

Import the following from Package Manager > Samples. Open the included Scene or Editor tool; follow any Setup step in its sample README.

- [Basic Usage](../Samples~/BasicUsage/README.md)

## Source map

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

[Detailed design](Design.en.md)
