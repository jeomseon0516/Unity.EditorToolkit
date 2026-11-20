# EditorToolkit: authoring tools and shared APIs

[한국어](Design.md) · [Installation](../README.en.md)

Shared APIs cover EditorWindows, SerializedProperty access, and type discovery. General Inspector
attributes/drawers belong to Attributes. Feature Editors depend on these APIs in one direction;
Runtime assemblies must not use UnityEditor APIs.

## IconCreator

Open Jeomseon/Icon Creator, load a Preset or set Size/Divide Count/source Sprites, Preview, then
Create Icon to save a PNG and Sprite metadata inside the project. Enable GenerateTmpSpriteAsset
in the Preset to also create a TMP SpriteAsset.

Size is cell size; Divide Count is the maximum cells per row. Actual output can be smaller for fewer
inputs; inspect the Preview. Null Sprites/over-capacity input are invalid. Editing input invalidates
the Preview. The window owns preview textures and releases them on recreation/close; saved assets
belong to the project.

SpriteAtlas optimizes runtime/build packing; this tool authors a fixed grid and TMP metadata.
Use the [sample Preset and four-color Sprites](../Samples~/BasicUsage/README.md) to verify output.

## Shared APIs

| Area | API | Contract |
| --- | --- | --- |
| Serialized values | `SerializedPropertyExtensions` | Choose accessors for the Unity serialized type |
| Enum metadata | `SerializedEnumData` / `GetEnumData` | Distinguish indices from actual values |
| Discovery | `EditorTypeDiscovery` | Editor TypeCache-based |
| Object graphs | `SerializedObjectGraphTraversal` | Respect Unity/managed reference boundaries |
| UI composition | `EditorDropdownController<T>`, `EditorGUILayoutActions` | Consumer owns callback/temporary-resource lifetimes |
| ScriptableObjects | `ScriptableObjectHandler` | Check asset paths, Undo, and project mutations |

Read [IconCreator](../Editor/Tool/IconCreator.cs), [Preset](../Editor/Tool/IconCreatorPreset.cs), then
[SerializedProperty helpers](../Editor/Extensions/SerializedPropertyExtensions.cs). UI Anchor Setter,
Object Naming Changer, and Bulk Component Remover change project objects; verify selection and Undo.
The sample EditorWindow demonstrates the basic shared API behavior.
