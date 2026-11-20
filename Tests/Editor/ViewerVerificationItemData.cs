using UnityEngine;

namespace Jeomseon.Unity.EditorToolkit.Tests
{
    // ScriptableObjectViewer 확인용 데이터. 기본 Inspector 표시를 확인한다.
    public sealed class ViewerVerificationItemData : ScriptableObject
    {
        [SerializeField] private string displayName;
        [SerializeField] private int price;
        [SerializeField] private Color tint = Color.white;
    }
}
