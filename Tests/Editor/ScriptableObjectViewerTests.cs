using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEditor.UIElements;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using Jeomseon.Unity.EditorToolkit.Editor.Window;

namespace Jeomseon.Unity.EditorToolkit.Tests
{
    internal sealed class ScriptableObjectViewerTests
    {
        private ScriptableObjectViewerVerificationWindow _window;

        [TearDown]
        public void TearDown()
        {
            if (_window != null)
            {
                _window.Close();
            }
        }

        [UnityTest]
        public IEnumerator OpenWindow_BuildsToolbarTabsAndInspectorHost()
        {
            _window = UnityEditor.EditorWindow.GetWindow<ScriptableObjectViewerVerificationWindow>();
            yield return null;
            yield return null;

            var root = _window.rootVisualElement;
            var labels = root.Query<ToolbarButton>().ToList().Select(button => button.text).ToArray();
            CollectionAssert.AreEqual(new[] { "Items", "Skills" }, labels);
            Assert.IsNotNull(root.Q<ScriptableObjectInspector>());
        }

        [UnityTest]
        public IEnumerator SelectTab_ListsAssetsOfThatType()
        {
            _window = UnityEditor.EditorWindow.GetWindow<ScriptableObjectViewerVerificationWindow>();
            yield return null;
            yield return null;

            var root = _window.rootVisualElement;
            var itemsTab = root.Query<ToolbarButton>().ToList().First(button => button.text == "Items");
            // Button은 NavigationSubmitEvent를 클릭과 같이 처리한다.
            using (var submit = NavigationSubmitEvent.GetPooled())
            {
                submit.target = itemsTab;
                itemsTab.SendEvent(submit);
            }
            yield return null;

            var scrollView = root.Q<ScriptableObjectScrollView>("SOScrollView");
            Assert.IsNotNull(scrollView);
            CollectionAssert.IsSupersetOf(
                scrollView.ScriptableObjects.Select(button => button.ScriptableObject.name).ToArray(),
                new[] { "ViewerItemA", "ViewerItemB" });
        }
    }
}
