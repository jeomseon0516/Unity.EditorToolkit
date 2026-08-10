#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Jeomseon.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

/// <summary>
/// Bulk Component Remover (통합판)
/// - 선택한 GameObject(들) + 본인 포함 자식을 순회하여 특정 컴포넌트를 일괄 제거
/// - 컴포넌트 지정: (1) MonoScript 드래그(컴포넌트만 허용), (2) 타입명 입력, (3) 컴포넌트 전용 타입 브라우저
/// - Missing Script 제거 전용 모드
/// - 레이어/태그 필터, Include Inactive, Search Children
/// - 미리보기 및 Undo 지원
/// 메뉴: Jeomseon > Tool > Bulk Component Remover
/// </summary>
namespace Jeomseon.Editor
{
    public class BulkComponentRemoverWindow : EditorWindow
    {
        // ----------------------------------------------------------------------------
        // 상태
        // ----------------------------------------------------------------------------
        private MonoScript _monoScript;                 // 컴포넌트 MonoScript(컴포넌트만 허용)
        private Type _resolvedType;                     // 최종 해석된 컴포넌트 타입(브라우저/MonoScript/문자열)
        private string _typeName = string.Empty;        // 직접 입력 타입명 (FQN 또는 간단 이름)

        private bool _missingOnly = false;              // Missing Script 제거 전용
        private bool _includeInactive = true;           // 비활성 포함
        private bool _searchInSelectionChildren = true; // 선택 오브젝트 자식 포함

        private bool _filterByLayer = false;
        private LayerMask _layerMask = ~0;              // 전체 레이어 기본값
        private bool _filterByTag = false;
        private string _tag = "Untagged";

        [Serializable]
        private class Match
        {
            public GameObject Go;
            public Component Component; // Missing Script일 때 null
            public bool IsMissing;
            public override string ToString() => IsMissing ? "[Missing Script]" : (this.Component ? this.Component.GetType().Name : "(null)");
        }

        private readonly List<Match> _matches = new();

        // ----------------------------------------------------------------------------
        // UI Toolkit 요소
        // ----------------------------------------------------------------------------
        private VisualElement _targetComponentSection;
        private Label _selectedTypeLabel;
        private ObjectField _monoScriptField;
        private HelpBox _monoWarningBox;
        private TextField _typeNameField;
        private VisualElement _layerMaskRow;
        private VisualElement _tagRow;
        private Label _selectionCountLabel;
        private HelpBox _selectionWarningBox;
        private Button _previewButton;
        private Button _removeButton;
        private Label _matchesCountLabel;
        private ScrollView _matchesScroll;

        // ----------------------------------------------------------------------------
        // 메뉴
        // ----------------------------------------------------------------------------
        [MenuItem("Jeomseon/Tool/Bulk Component Remover")]
        public static void Open()
        {
            var wnd = GetWindow<BulkComponentRemoverWindow>(true, "Bulk Component Remover");
            wnd.minSize = new Vector2(560, 480);
            wnd.Show();
        }

        // ----------------------------------------------------------------------------
        // GUI
        // ----------------------------------------------------------------------------
        private void CreateGUI()
        {
            var root = rootVisualElement;
            root.style.paddingLeft = 8;
            root.style.paddingRight = 8;
            root.style.paddingTop = 8;
            root.style.paddingBottom = 8;

            root.Add(BuildModeSection());
            _targetComponentSection = BuildTargetComponentSection();
            root.Add(_targetComponentSection);
            root.Add(BuildScopeAndFiltersSection());
            root.Add(BuildSelectionInfoSection());
            root.Add(BuildActionsSection());
            root.Add(BuildMatchesSection());

            RefreshTargetComponentEnabled();
            InvalidatePreview();
        }

        private static void AddBoundToggle(VisualElement section, string label, bool initialValue, string tooltip, Action<bool> onChanged)
        {
            var toggle = new Toggle(label)
            {
                value = initialValue,
                tooltip = tooltip
            };
            toggle.RegisterValueChangedCallback(evt => onChanged(evt.newValue));
            section.Add(toggle);
        }

        private VisualElement BuildModeSection()
        {
            var section = new VisualElement();
            section.Add(new Label("Mode") { style = { unityFontStyleAndWeight = FontStyle.Bold } });

            AddBoundToggle(section, "Missing Script Cleanup Only", _missingOnly,
                "Missing Script만 제거합니다 (타입 지정 무시)", value =>
                {
                    _missingOnly = value;
                    RefreshTargetComponentEnabled();
                    InvalidatePreview();
                });
            return section;
        }

        private VisualElement BuildTargetComponentSection()
        {
            var section = new VisualElement();
            section.Add(new Label("Target Component") { style = { unityFontStyleAndWeight = FontStyle.Bold } });

            var browseRow = new VisualElement { style = { flexDirection = FlexDirection.Row } };
            var browseButton = new Button(BrowseComponentTypes)
            {
                text = "Browse Component Types...",
                tooltip = "컴포넌트 타입 선택 팝업"
            };
            browseButton.style.width = 220;
            browseRow.Add(browseButton);

            _selectedTypeLabel = new Label();
            browseRow.Add(_selectedTypeLabel);
            section.Add(browseRow);

            _monoScriptField = new ObjectField("MonoScript")
            {
                objectType = typeof(MonoScript),
                allowSceneObjects = false,
                tooltip = "Component 타입만 허용",
                value = _monoScript
            };
            _monoScriptField.RegisterValueChangedCallback(evt => OnMonoScriptPicked(evt.newValue as MonoScript));
            section.Add(_monoScriptField);

            _monoWarningBox = new HelpBox("선택한 MonoScript가 Component 타입이 아닙니다.", HelpBoxMessageType.Warning)
            {
                style = { display = DisplayStyle.None }
            };
            section.Add(_monoWarningBox);

            var typeNameRow = new VisualElement { style = { flexDirection = FlexDirection.Row } };
            _typeNameField = new TextField("Type Name")
            {
                value = _typeName,
                tooltip = "예: BoxCollider, UnityEngine.BoxCollider, Namespace.MyComponent",
                style = { flexGrow = 1 }
            };
            _typeNameField.RegisterValueChangedCallback(evt =>
            {
                _typeName = evt.newValue;
                _resolvedType = null;
                _monoScript = null;
                _monoWarningBox.style.display = DisplayStyle.None;
                _monoScriptField.SetValueWithoutNotify(null);
                RefreshSelectedType();
                InvalidatePreview();
            });
            typeNameRow.Add(_typeNameField);

            var findTypeButton = new Button(() =>
            {
                _resolvedType = ResolveType();
                RefreshSelectedType();
                InvalidatePreview();
            }) { text = "Find Type" };
            findTypeButton.style.width = 90;
            typeNameRow.Add(findTypeButton);
            section.Add(typeNameRow);

            return section;
        }

        private VisualElement BuildScopeAndFiltersSection()
        {
            var section = new VisualElement();
            section.Add(new Label("Scope & Filters") { style = { unityFontStyleAndWeight = FontStyle.Bold } });

            AddBoundToggle(section, "Include Inactive", _includeInactive,
                "비활성 오브젝트 포함", value =>
                {
                    _includeInactive = value;
                    InvalidatePreview();
                });

            AddBoundToggle(section, "Search Children", _searchInSelectionChildren,
                "선택 오브젝트의 자식 포함", value =>
                {
                    _searchInSelectionChildren = value;
                    InvalidatePreview();
                });

            AddBoundToggle(section, "Filter by Layer", _filterByLayer,
                "특정 레이어만 대상", value =>
                {
                    _filterByLayer = value;
                    _layerMaskRow.style.display = _filterByLayer ? DisplayStyle.Flex : DisplayStyle.None;
                    InvalidatePreview();
                });

            _layerMaskRow = new VisualElement
            {
                style = { display = _filterByLayer ? DisplayStyle.Flex : DisplayStyle.None }
            };
            var layerMaskField = new LayerMaskField("Layer Mask", _layerMask.value);
            layerMaskField.RegisterValueChangedCallback(evt =>
            {
                _layerMask = evt.newValue;
                InvalidatePreview();
            });
            _layerMaskRow.Add(layerMaskField);
            section.Add(_layerMaskRow);

            AddBoundToggle(section, "Filter by Tag", _filterByTag,
                "특정 태그만 대상", value =>
                {
                    _filterByTag = value;
                    _tagRow.style.display = _filterByTag ? DisplayStyle.Flex : DisplayStyle.None;
                    InvalidatePreview();
                });

            _tagRow = new VisualElement
            {
                style = { display = _filterByTag ? DisplayStyle.Flex : DisplayStyle.None }
            };
            var tagField = new TagField("Tag", _tag);
            tagField.RegisterValueChangedCallback(evt =>
            {
                _tag = evt.newValue;
                InvalidatePreview();
            });
            _tagRow.Add(tagField);
            section.Add(_tagRow);

            return section;
        }

        private VisualElement BuildSelectionInfoSection()
        {
            var section = new VisualElement();

            _selectionCountLabel = new Label();
            section.Add(_selectionCountLabel);

            _selectionWarningBox = new HelpBox(string.Empty, HelpBoxMessageType.Info)
            {
                style = { display = DisplayStyle.None }
            };
            section.Add(_selectionWarningBox);

            return section;
        }

        private VisualElement BuildActionsSection()
        {
            var row = new VisualElement { style = { flexDirection = FlexDirection.Row } };

            _previewButton = new Button(() =>
            {
                Preview();
                RefreshMatches();
                RefreshSelectionInfo();
                RefreshInteractable();
            }) { text = "Preview Matches" };
            row.Add(_previewButton);

            _removeButton = new Button(() =>
            {
                RemoveNow();
                RefreshMatches();
                RefreshInteractable();
            })
            {
                text = "Remove Components",
                tooltip = "미리보기 결과를 기준으로 제거"
            };
            row.Add(_removeButton);

            return row;
        }

        private VisualElement BuildMatchesSection()
        {
            var section = new VisualElement();

            _matchesCountLabel = new Label { style = { unityFontStyleAndWeight = FontStyle.Bold } };
            section.Add(_matchesCountLabel);

            _matchesScroll = new ScrollView { style = { flexGrow = 1 } };
            section.Add(_matchesScroll);

            return section;
        }

        private void OnMonoScriptPicked(MonoScript picked)
        {
            _monoWarningBox.style.display = DisplayStyle.None;
            if (picked == null)
            {
                _monoScript = null;
                _resolvedType = null;
            }
            else
            {
                var cls = picked.GetClass();
                if (cls != null && typeof(Component).IsAssignableFrom(cls))
                {
                    _monoScript = picked;
                    _resolvedType = null; // MonoScript 우선 사용 시 타입 직접 선택은 초기화
                }
                else
                {
                    _monoScript = null;
                    _monoWarningBox.style.display = DisplayStyle.Flex;
                }
            }

            RefreshSelectedType();
            InvalidatePreview();
        }

        private void BrowseComponentTypes()
        {
            Type pickedType = ComponentTypePicker.ShowPicker();
            if (pickedType == null)
                return;

            _resolvedType = pickedType;
            _typeName = pickedType.FullName ?? pickedType.Name;
            _monoScript = null;
            _monoWarningBox.style.display = DisplayStyle.None;
            _typeNameField.SetValueWithoutNotify(_typeName);
            _monoScriptField.SetValueWithoutNotify(null);
            RefreshSelectedType();
            InvalidatePreview();
        }

        private void RefreshSelectedType()
        {
            _selectedTypeLabel.text = _resolvedType != null ? $"Selected: {_resolvedType.FullName}" : string.Empty;
        }

        private void RefreshSelectionInfo()
        {
            var selected = Selection.gameObjects;
            if (selected == null || selected.Length == 0)
            {
                _selectionCountLabel.text = string.Empty;
                _selectionWarningBox.text = "하나 이상의 GameObject를 선택하세요.";
                _selectionWarningBox.messageType = HelpBoxMessageType.Info;
                _selectionWarningBox.style.display = DisplayStyle.Flex;
                return;
            }

            _selectionCountLabel.text = $"Selected Objects: {selected.Length}";

            if (!CanSearch())
            {
                _selectionWarningBox.text = _missingOnly
                    ? "Missing Script 모드에서 Preview를 실행하세요."
                    : "제거할 컴포넌트 타입을 지정하세요 (MonoScript/Type Name/브라우저).";
                _selectionWarningBox.messageType = HelpBoxMessageType.Warning;
                _selectionWarningBox.style.display = DisplayStyle.Flex;
            }
            else
            {
                _selectionWarningBox.style.display = DisplayStyle.None;
            }
        }

        private void RefreshInteractable()
        {
            _previewButton.SetEnabled(CanSearch());
            _removeButton.SetEnabled(_matches.Count > 0);
        }

        private void RefreshTargetComponentEnabled()
        {
            _targetComponentSection.SetEnabled(!_missingOnly);
        }

        private void RefreshMatches()
        {
            _matchesCountLabel.text = $"Matches: {_matches.Count}";
            _matchesScroll.Clear();

            foreach (var match in _matches)
            {
                var row = new VisualElement { style = { flexDirection = FlexDirection.Row } };

                var goField = new ObjectField { objectType = typeof(GameObject), value = match.Go, style = { flexGrow = 1 } };
                row.Add(goField);

                row.Add(new Label(match.ToString()) { style = { width = 200 } });

                var componentField = new ObjectField { objectType = typeof(Component), value = match.Component, style = { flexGrow = 1 } };
                componentField.SetEnabled(false);
                row.Add(componentField);

                _matchesScroll.Add(row);
            }
        }

        private void InvalidatePreview()
        {
            _matches.Clear();
            RefreshMatches();
            RefreshSelectionInfo();
            RefreshInteractable();
        }

        // ----------------------------------------------------------------------------
        // 검색/해석/제거 로직
        // ----------------------------------------------------------------------------
        private bool CanSearch()
        {
            if (_missingOnly)
                return true;

            if (_resolvedType != null &&
                typeof(Component).IsAssignableFrom(_resolvedType))
            {
                return true;
            }

            if (_monoScript != null)
            {
                Type type = _monoScript.GetClass();
                if (type != null &&
                    typeof(Component).IsAssignableFrom(type))
                {
                    return true;
                }
            }

            return !string.IsNullOrWhiteSpace(_typeName);
        }

        private Type ResolveType()
        {
            // 1) 팝업으로 이미 선택한 타입이 있으면 최우선 사용
            if (_resolvedType != null && typeof(Component).IsAssignableFrom(_resolvedType))
                return _resolvedType;

            // 2) MonoScript가 지정된 경우 (Component만 허용 검증됨)
            if (_monoScript != null)
            {
                var scriptType = _monoScript.GetClass();
                if (scriptType != null && typeof(Component).IsAssignableFrom(scriptType)) return scriptType;
            }

            // 3) 문자열로 입력된 타입명 해석
            var name = _typeName?.Trim();
            if (string.IsNullOrEmpty(name)) return null;

            List<Type> componentTypes = EditorTypeDiscovery
                .GetConcreteTypesDerivedFrom<Component>()
                .ToList();

            // FQN 정확 매치
            var exact = componentTypes.FirstOrDefault(type =>
                string.Equals(type.FullName, name, StringComparison.Ordinal));
            if (exact != null) return exact;

            // 간단 이름 매치/부분 매치 후보
            List<Type> candidates = componentTypes
                .Where(type =>
                    string.Equals(type.Name, name, StringComparison.Ordinal) ||
                    type.Name.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0)
                .OrderBy(type => type.FullName)
                .ToList();

            return candidates.Count switch
            {
                0 => null,
                1 => candidates[0],
                _ => SelectCandidate()
            };

            Type SelectCandidate()
            {
                string[] options = candidates
                    .Select(type => type.FullName ?? type.Name)
                    .ToArray();

                int choice = PopupListWindow.Show(
                    "Select Component Type",
                    options);

                return choice >= 0 && choice < candidates.Count
                    ? candidates[choice]
                    : null;
            }
        }

        private void Preview()
        {
            if (!_missingOnly)
            {
                _resolvedType = ResolveType();
                RefreshSelectedType();
                if (_resolvedType == null)
                {
                    _matches.Clear();
                    EditorUtility.DisplayDialog(
                        "Type Not Found",
                        "입력한 Component 타입을 찾을 수 없습니다.",
                        "OK");
                    return;
                }
            }

            _matches.Clear();

            var selection = Selection.gameObjects;
            if (selection == null || selection.Length == 0)
            {
                EditorUtility.DisplayDialog("No Selection", "하나 이상의 GameObject를 선택하세요.", "OK");
                return;
            }

            HashSet<GameObject> targets = new();
            foreach (GameObject root in selection)
            {
                if (root == null)
                    continue;

                foreach (GameObject target in CollectTargets(
                             root,
                             _searchInSelectionChildren,
                             _includeInactive))
                {
                    targets.Add(target);
                }
            }

            foreach (GameObject go in targets)
            {
                if (!PassesFilters(go))
                    continue;

                if (_missingOnly)
                {
                    Component[] components = go.GetComponents<Component>();
                    foreach (Component component in components)
                    {
                        if (component == null)
                        {
                            _matches.Add(new Match
                                { Go = go, Component = null, IsMissing = true });
                        }
                    }
                }
                else
                {
                    Component[] components = go.GetComponents(_resolvedType);
                    foreach (Component component in components)
                    {
                        if (component == null) continue;

                        _matches.Add(new Match
                            { Go = go, Component = component, IsMissing = false });
                    }
                }
            }
        }

        private bool PassesFilters(GameObject go)
        {
            if (_filterByLayer)
            {
                if (((1 << go.layer) & _layerMask.value) == 0) return false;
            }
            if (_filterByTag && !go.CompareTag(_tag))
            {
                return false;
            }
            return true;
        }

        private void RemoveNow()
        {
            if (_matches.Count == 0)
            {
                EditorUtility.DisplayDialog("No Matches", "제거할 컴포넌트가 없습니다. 먼저 Preview 하세요.", "OK");
                return;
            }

            var title = _missingOnly ? "Remove Missing Scripts" : "Remove Components";
            if (!EditorUtility.DisplayDialog(title,
                    $"총 {_matches.Count}개의 대상이 검색되었습니다. 되돌리기는 Undo(Ctrl/Cmd+Z)로 가능합니다.",
                    "Proceed", "Cancel"))
            {
                return;
            }

            Undo.IncrementCurrentGroup();
            int group = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName(title);

            HashSet<Scene> affectedScenes = _matches
                .Where(match => match?.Go != null && match.Go.scene.IsValid())
                .Select(match => match.Go.scene)
                .ToHashSet();

            int removed = 0;
            if (_missingOnly)
            {
                var perGo = _matches
                    .Select(m => m.Go)
                    .Distinct();

                foreach (var go in perGo)
                {
                    if (go == null) continue;
                    int before = go.GetComponents<Component>().Count(c => c == null);
                    if (before == 0) continue;
                    Undo.RegisterFullObjectHierarchyUndo(go, "Remove Missing Scripts");
                    GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                    int after = go.GetComponents<Component>().Count(c => c == null);
                    removed += Mathf.Max(0, before - after);
                }
            }
            else
            {
                foreach (var m in _matches.Where(m => m != null && m.Component != null))
                {
                    Undo.DestroyObjectImmediate(m.Component);
                    removed++;
                }
            }

            Undo.CollapseUndoOperations(group);

            if (!Application.isPlaying)
            {
                affectedScenes
                    .Where(scene => scene.isLoaded)
                    .ForEach(scene => EditorSceneManager.MarkSceneDirty(scene));
            }

            EditorUtility.DisplayDialog("Done", $"Removed {removed} component(s).", "OK");
            _matches.Clear();
        }

        // ----------------------------------------------------------------------------
        // Helpers
        // ----------------------------------------------------------------------------
        private static IEnumerable<GameObject> CollectTargets(GameObject root, bool includeChildren, bool includeInactive)
        {
            if (root == null) yield break;
            yield return root;
            if (!includeChildren) yield break;
            foreach (var t in root.GetComponentsInChildren<Transform>(includeInactive))
            {
                if (t == null) continue;
                if (t.gameObject == root) continue; // root는 이미 포함됨
                yield return t.gameObject;
            }
        }

        private void OnSelectionChange()
        {
            InvalidatePreview();
        }

        // 컴포넌트 타입 전용 선택 팝업 (TypeCache 사용)
        private static class ComponentTypePicker
        {
            public static Type ShowPicker()
            {
                List<Type> all = EditorTypeDiscovery
                    .GetConcreteTypesDerivedFrom<Component>()
                    .Where(type => type.IsPublic || type.IsNestedPublic)
                    .OrderBy(t => t.Namespace)
                    .ThenBy(t => t.Name)
                    .ToList();

                var display = all.Select(t => string.IsNullOrEmpty(t.Namespace) ? t.Name : $"{t.Namespace}.{t.Name}").ToArray();
                int idx = PopupListWindow.Show("Select Component Type", display);
                if (idx >= 0 && idx < all.Count) return all[idx];
                return null;
            }
        }

        // 검색 가능한 팝업 리스트 유틸
        private class PopupListWindow : EditorWindow
        {
            private string[] _options;
            private string _filter = string.Empty;
            private int _picked = -1;
            private ScrollView _list;

            public static int Show(string title, string[] options)
            {
                var wnd = CreateInstance<PopupListWindow>();
                wnd.titleContent = new GUIContent(title);
                wnd._options = options ?? Array.Empty<string>();
                wnd.position = new Rect(Screen.width / 2f, Screen.height / 2f, 420f, Mathf.Min(420f, 24f * (options?.Length ?? 1) + 56f));
                wnd.ShowModal();
                return wnd._picked;
            }

            private void CreateGUI()
            {
                var root = rootVisualElement;
                root.style.paddingLeft = 4;
                root.style.paddingRight = 4;
                root.style.paddingTop = 4;
                root.style.paddingBottom = 4;

                var search = new ToolbarSearchField();
                search.RegisterValueChangedCallback(evt =>
                {
                    _filter = evt.newValue;
                    RefreshList();
                });
                root.Add(search);

                _list = new ScrollView { style = { flexGrow = 1 } };
                root.Add(_list);

                RefreshList();
            }

            private void RefreshList()
            {
                _list.Clear();

                IEnumerable<(int index, string option)> entries = _options.Select((option, index) => (index, option));
                if (!string.IsNullOrEmpty(_filter))
                {
                    entries = entries.Where(entry =>
                        entry.option.IndexOf(_filter, StringComparison.OrdinalIgnoreCase) >= 0);
                }

                foreach (var (index, option) in entries)
                {
                    var button = new Button(() =>
                    {
                        _picked = index;
                        Close();
                    }) { text = option };
                    button.style.unityTextAlign = TextAnchor.MiddleLeft;
                    _list.Add(button);
                }
            }
        }
    }
}
#endif
