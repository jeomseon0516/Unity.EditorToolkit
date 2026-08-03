using Jeomseon.Attribute;
using UnityEngine;

namespace Jeomseon.Samples.EditorToolkit
{
    public sealed class InspectorInjectionSample : MonoBehaviour
    {
        [SerializeField] private string _message = "Inspector Injection";
        [ReadOnly, SerializeField] private int _changeCount;
        [ReadOnly, SerializeField] private int _buttonClickCount;

        [OnChangedValueForMethod(nameof(_message))]
        private void OnMessageChanged()
        {
            _changeCount++;
        }

        [InspectorButton("Injection 버튼 실행")]
        private void InvokeFromInspector()
        {
            Debug.Log($"{_message} - 버튼 클릭 횟수: {++_buttonClickCount}", this);
        }
    }
}
