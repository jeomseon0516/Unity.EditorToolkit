# Jeomseon Unity Editor Toolkit

## UI Toolkit migration status

As part of P2-01, `UIAnchorSetter` and `ObjectNamingChanger` were migrated to the
UI Toolkit `CreateGUI()` model. The existing `IMGUIHelper` remains for compatibility;
`BulkComponentRemoverWindow`, `LoadableScriptableObjectDrawer`, and SceneView-based UI
remain future migration targets.

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
