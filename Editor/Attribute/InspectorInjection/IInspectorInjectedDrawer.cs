#if UNITY_EDITOR
using System;

namespace Jeomseon.Attribute.Editor
{
    using UnityObjectEditor = UnityEditor.Editor;

    /// <summary>
    /// Inspector Injection 영역에 추가할 기능의 공통 계약입니다.
    /// Unity 내부 구조 접근은 백엔드가 담당하고 Drawer는 Editor만 전달받습니다.
    /// </summary>
    internal interface IInspectorInjectedDrawer : IDisposable
    {
        void OnEnable(UnityObjectEditor editor);
        void OnInspectorGUI(UnityObjectEditor editor);
    }
}
#endif
