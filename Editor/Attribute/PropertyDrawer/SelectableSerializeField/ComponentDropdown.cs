#if UNITY_EDITOR
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Jeomseon.Scope;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace Jeomseon.Attribute.Editor
{
    internal sealed class ComponentDropdown : TreeView
    {
        private sealed class ComponentDropdownPopupContent : PopupWindowContent
        {
            private readonly ComponentDropdown _dropdown;
            private readonly SearchField _searchField;
            private readonly GUIStyle _labelStyle;

            // ✅ horizontal-only scroll state
            private float _hScroll;

            public ComponentDropdownPopupContent(ComponentDropdown dropdown)
            {
                _dropdown = dropdown;
                _labelStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    alignment = TextAnchor.MiddleCenter
                };
                _searchField = new SearchField();
            }

            public override Vector2 GetWindowSize()
            {
                return new Vector2(300f, Mathf.Clamp(_dropdown.ItemCount * _dropdown.rowHeight, 200, 800));
            }

            public override void OnGUI(Rect rect)
            {
                float voidWidth = rect.width * 0.1f;
                rect.y += 3.0f;

                Rect searchRect = new Rect(
                    rect.x + voidWidth * 0.5f,
                    rect.y,
                    rect.width - voidWidth,
                    EditorGUIUtility.singleLineHeight);

                _dropdown.searchString = _searchField.OnGUI(searchRect, _dropdown.searchString);

                EditorGUI.LabelField(
                    new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 1.35f, rect.width, EditorGUIUtility.singleLineHeight),
                    "Nested Object",
                    _labelStyle);

                Rect treeViewRect = new Rect(
                    rect.x,
                    rect.y + EditorGUIUtility.singleLineHeight * 2.5f,
                    rect.width,
                    rect.height - EditorGUIUtility.singleLineHeight * 2.5f);

                // ✅ horizontal-only outer scroll:
                // - Outer scroll handles ONLY X.
                // - TreeView keeps handling Y internally.
                float contentWidth = _dropdown.GetContentWidth(treeViewRect.width);

                // viewRect: visible area (same as treeViewRect)
                Rect viewRect = treeViewRect;

                // contentRect height == viewRect height -> prevents vertical scrollbar on outer scroll
                Rect contentRect = new Rect(0f, 0f, contentWidth, viewRect.height);

                // Horizontal scrollbar height reserved at bottom (optional but helps layout)
                // We'll use GUI.BeginGroup for positioning and a separate horizontal scrollbar control.
                // This avoids BeginScrollView creating a vertical scrollbar at all.
                drawHorizontalOnlyTree(viewRect, contentRect, contentWidth);
            }

            private void drawHorizontalOnlyTree(Rect viewRect, Rect contentRect, float contentWidth)
            {
                // We implement horizontal-only scrolling manually:
                // 1) Clip to viewRect
                // 2) Offset drawing by -_hScroll
                // 3) Draw a bottom horizontal scrollbar when needed

                const float scrollbarHeight = 16f;

                bool needH = contentWidth > viewRect.width + 0.5f;

                Rect treeArea = viewRect;
                Rect hbarRect = default;

                if (needH)
                {
                    // Reserve space for the horizontal scrollbar so it doesn't overlap rows
                    treeArea.height -= scrollbarHeight;
                    hbarRect = new Rect(viewRect.x, viewRect.yMax - scrollbarHeight, viewRect.width, scrollbarHeight);
                }

                // Clip the drawing region to the tree area
                GUI.BeginGroup(treeArea);
                {
                    // Shift content left/right
                    Rect innerRect = new Rect(-_hScroll, 0f, contentWidth, treeArea.height);

                    // Draw TreeView in the shifted coordinates.
                    _dropdown.OnGUI(innerRect);
                }
                GUI.EndGroup();

                if (needH)
                {
                    float maxScroll = Mathf.Max(0f, contentWidth - viewRect.width);
                    _hScroll = GUI.HorizontalScrollbar(hbarRect, _hScroll, viewRect.width, 0f, maxScroll + viewRect.width);
                    _hScroll = Mathf.Clamp(_hScroll, 0f, maxScroll);
                }
                else
                {
                    _hScroll = 0f;
                }
            }
        }

        public int ItemCount
        {
            get
            {
                return countItems(rootItem);

                static int countItems(TreeViewItem item)
                {
                    return item.children?.Aggregate(1, (count, child) => count + countItems(child)) ?? 1;
                }
            }
        }

        private readonly Action<GameObject> _onSelected;
        private readonly GameObject _rootObject;
        private readonly Type _defaultType;
        private readonly Type _filterType;
        private readonly Texture2D _gameObjectImage;
        private readonly Dictionary<int, GameObject> _itemsMap = new();
        private readonly ComponentDropdownPopupContent _content;
        private Texture2D _targetTexture = null;

        private float _cachedContentWidth = -1f;
        private string _cachedSearchString = null;

        public ComponentDropdown(TreeViewState state, GameObject rootObject, Type filterType, Action<GameObject> onSelected) : base(state)
        {
            _rootObject = rootObject;
            _filterType = filterType;
            _onSelected = onSelected;

            rowHeight *= 1.5f;
            _defaultType = typeof(GameObject);

            _gameObjectImage = EditorGUIUtility.ObjectContent(null, _defaultType).image as Texture2D;
            _content = new ComponentDropdownPopupContent(this);
        }

        protected override TreeViewItem BuildRoot()
        {
            _itemsMap.Clear();
            _cachedContentWidth = -1f;
            _cachedSearchString = null;

            TreeViewItem root = new TreeViewItem(0, -1, "root");

            if (_filterType == typeof(GameObject))
            {
                addGameObjectToDropdown(root, _rootObject);
            }
            else
            {
                TreeViewItem treeRootItem = createItem(_rootObject, 0);

                Dictionary<GameObject, TreeViewItem> visited = new Dictionary<GameObject, TreeViewItem>
                {
                    { _rootObject, treeRootItem }
                };

                foreach (Component component in _rootObject.GetComponentsInChildren(_filterType, true))
                {
                    _itemsMap[component.gameObject.GetInstanceID()] = component.gameObject;
                    addComponentToDropdown(component.gameObject, visited);
                }

                setDepth(treeRootItem);

                static void setDepth(TreeViewItem item, int depth = 0)
                {
                    item.depth = depth;

                    if (item.hasChildren)
                    {
                        foreach (TreeViewItem treeViewItem in item.children)
                            setDepth(treeViewItem, depth + 1);
                    }
                }

                root.AddChild(treeRootItem);
            }

            return root;
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            IList<TreeViewItem> rows = base.BuildRows(root);

            if (!string.IsNullOrEmpty(searchString))
            {
                rows = rows
                    .Where(item => item.displayName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
            }

            if (_cachedSearchString != searchString)
            {
                _cachedSearchString = searchString;
                _cachedContentWidth = -1f;
            }

            return rows;
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            TreeViewItem item = args.item;

            Rect labelRect = new Rect(
                string.IsNullOrEmpty(searchString) ?
                    depthIndentWidth + args.item.depth * depthIndentWidth :
                    0f,
                args.rowRect.y + (args.rowRect.height - EditorGUIUtility.singleLineHeight) * 0.5f,
                args.rowRect.width,
                EditorGUIUtility.singleLineHeight);

            GUIContent guiContent = new GUIContent(item.displayName, getIconForItem(item));
            EditorGUI.LabelField(labelRect, guiContent);
        }

        protected override void DoubleClickedItem(int id)
        {
            if (!_itemsMap.TryGetValue(id, out GameObject @object)) return;

            _content.editorWindow?.Close();
            _onSelected?.Invoke(@object);
        }

        public void Show(Rect rect)
        {
            Reload();
            ExpandAll();
            PopupWindow.Show(rect, _content);
        }

        public float GetContentWidth(float minWidth)
        {
            if (_cachedContentWidth >= 0f)
                return Mathf.Max(minWidth, _cachedContentWidth);

            const float padding = 70f; // icon + spacing + safety
            float max = minWidth;

            IList<TreeViewItem> rows = GetRows();
            GUIStyle style = EditorStyles.label;

            for (int i = 0; i < rows.Count; i++)
            {
                TreeViewItem item = rows[i];

                float indent = string.IsNullOrEmpty(searchString)
                    ? (depthIndentWidth + item.depth * depthIndentWidth)
                    : 0f;

                Vector2 size = style.CalcSize(new GUIContent(item.displayName));
                float width = indent + size.x + padding;
                if (width > max) max = width;
            }

            _cachedContentWidth = max;
            return max;
        }

        private Texture2D getIconForItem(TreeViewItem item)
        {
            if (_filterType == _defaultType ||
                !_itemsMap.TryGetValue(item.id, out GameObject gameObject) ||
                !gameObject.TryGetComponent(_filterType, out Component component))
            {
                return _gameObjectImage;
            }

            if (!_targetTexture)
                _targetTexture = EditorGUIUtility.ObjectContent(component, _filterType).image as Texture2D;

            return _targetTexture;
        }

        private void addComponentToDropdown(GameObject selectedObject, Dictionary<GameObject, TreeViewItem> visited, TreeViewItem prevItem = null)
        {
            while (true)
            {
                if (visited.TryGetValue(selectedObject, out TreeViewItem advancedDropdownItem))
                {
                    if (prevItem is not null)
                        advancedDropdownItem.AddChild(prevItem);

                    return;
                }

                TreeViewItem gameObjectItem = createItem(selectedObject, 0);
                visited.Add(selectedObject, gameObjectItem);

                if (prevItem is not null)
                    gameObjectItem.AddChild(prevItem);

                selectedObject = selectedObject.transform.parent.gameObject;
                prevItem = gameObjectItem;
            }
        }

        private void addGameObjectToDropdown(TreeViewItem parent, GameObject gameObject, int depth = 0)
        {
            TreeViewItem gameObjectItem = createItem(gameObject, depth);

            parent.AddChild(gameObjectItem);
            _itemsMap[gameObjectItem.id] = gameObject;

            foreach (Transform child in gameObject.transform)
                addGameObjectToDropdown(gameObjectItem, child.gameObject, depth + 1);
        }

        private TreeViewItem createItem(GameObject gameObject, int depth)
        {
            return new TreeViewItem(gameObject.GetInstanceID(), depth, buildString(gameObject));
        }

        private string buildString(GameObject go)
        {
            using StringBuilderPoolScope scope = new StringBuilderPoolScope();
            StringBuilder builder = scope.Get();

            builder.Append(go.name);
            builder.Append(" (");
            builder.Append(_filterType == _defaultType || go.TryGetComponent(_filterType, out Component _)
                ? _filterType.Name
                : _defaultType.Name);
            builder.Append(")");

            string name = builder.ToString();
            builder.Clear();
            return name;
        }
    }
}
#endif
