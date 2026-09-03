# Jeomseon Unity Editor Toolkit

Shared editor APIs, editor windows, and ScriptableObject authoring tools.

## Install via OpenUPM

Register the OpenUPM scoped registry once in your project's `Packages/manifest.json`.

```json
{
  "scopedRegistries": [
    {
      "name": "OpenUPM",
      "url": "https://package.openupm.com",
      "scopes": [
        "com.jeomseon"
      ]
    }
  ],
  "dependencies": {
    "com.jeomseon.unity.editor-toolkit": "0.7.3"
  }
}
```

## Install via Git URL

Enter the following URL in Unity Package Manager's `Install package from git URL`.

```text
https://github.com/jeomseon0516/Unity.EditorToolkit.git#v0.7.3
```

## UI Toolkit migration status

As part of P2-01, `UIAnchorSetter`, `ObjectNamingChanger`, `BulkComponentRemoverWindow`,
and `LoadableScriptableObjectDrawer` were migrated to the UI Toolkit
(`CreateGUI()`/`CreatePropertyGUI()`) model. The former `IMGUIHelper`, whose
responsibilities had become an unclear grab-bag, was removed; the layout helpers still
in active use were split out into `EditorGUILayoutActions` and `GUIStyleTexture` with
clearer names and ownership. `IconCreator` now uses `CreateGUI()` and `ListView`.
The generic SceneView IMGUI window was removed, and consumers now own their native
Unity `Overlay` implementations.

## Icon Creator

`IconCreator` (`Jeomseon/Icon Creator`) combines several individual icon sprites into a
fixed-grid texture and, from that, generates a TextMeshPro `TMP_SpriteAsset` (for inline
sprites). Unity's Sprite Atlas system (`com.unity.2d.sprite`) is a separate optimization
tool that automatically packs scattered sprites at build/runtime to reduce draw calls; it
has no notion of fixed-grid layout and does not produce a `TMP_SpriteAsset`. TextMeshPro's
own Sprite Asset Creator likewise assumes an atlas and sprite metadata already exist, so
neither Unity nor TMP automates the step of combining N loose source icons into a grid.
`IconCreatorPreset` (a `ScriptableObject`) lets you save and reuse the size, divide count,
and default icon list. `Samples~/BasicUsage` includes 4 verification sample sprites and a
matching preset.

## SerializedProperty utilities

`SerializedPropertyExtensions.GetPropertyValue()` returns most property values through
`SerializedProperty.boxedValue`. Types with `boxedValue` limitations, such as `LayerMask`,
`AnimationCurve`, `Gradient`, and array metadata, use their dedicated accessors.

Enum properties are returned as `SerializedEnumData`, preserving `EnumType`, `Value`,
`Index`, `Name`, and `DisplayName`. Use `SerializedPropertyExtensions.GetEnumData()` when
enum metadata is needed directly.

Shared editor APIs, editor windows, and ScriptableObject authoring tools.

Feature-independent inspector attribute declarations and their editor implementations
belong to `com.jeomseon.unity.attributes`. Feature-specific attributes and drawers
belong to their feature packages.
