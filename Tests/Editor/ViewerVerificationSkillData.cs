using UnityEngine;

namespace Jeomseon.Unity.EditorToolkit.Tests
{
    // ScriptableObjectViewer 확인용 두 번째 타입. 툴바 탭 전환을 확인한다.
    public sealed class ViewerVerificationSkillData : ScriptableObject
    {
        [SerializeField, TextArea] private string description;
        [SerializeField, Min(0f)] private float cooldown = 1f;
        [SerializeField] private bool isPassive;
    }
}
