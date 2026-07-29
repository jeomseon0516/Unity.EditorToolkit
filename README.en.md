# Jeomseon Unity Editor Toolkit

`InspectorButtonAttribute` uses a versioned Inspector Injection backend so it can
append buttons after the inspector body even when another custom editor is active.
The Unity 2022.3–2023 and Unity 6 implementations are compiled separately. Because
this feature reads non-public InspectorWindow/InspectorElement structure, a Unity
patch can break the injection. Failure is isolated to this feature and
`InspectorButtonGUI.Draw(this)` remains available as an explicit stable fallback.

`OnChangedValueForMethodAttribute` observes Unity's global Undo modification stream
instead of replacing the active custom editor. It therefore works with other custom
editors when they edit through `SerializedObject`/`SerializedProperty` or call
`Undo.RecordObject`. Direct field assignments that are not recorded by Undo cannot
be observed reliably through Unity's public editor APIs.

Custom drawers for the lightweight Attributes package, editor windows, and ScriptableObject authoring tools.

Localization-specific attributes and drawers belong to `com.jeomseon.unity.localization`.
