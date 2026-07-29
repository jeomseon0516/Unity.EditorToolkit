# Jeomseon Unity Editor Toolkit

`OnChangedValueForMethodAttribute` observes Unity's global Undo modification stream
instead of replacing the active custom editor. It therefore works with other custom
editors when they edit through `SerializedObject`/`SerializedProperty` or call
`Undo.RecordObject`. Direct field assignments that are not recorded by Undo cannot
be observed reliably through Unity's public editor APIs.

Custom drawers for the lightweight Attributes package, editor windows, and ScriptableObject authoring tools.

Localization-specific attributes and drawers belong to `com.jeomseon.unity.localization`.
