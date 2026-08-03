#if UNITY_EDITOR
using System;

namespace Jeomseon.Attribute.Editor
{
    /// <summary>
    /// Trigger 종류별 매개변수 정책을 Registry에 제공하는 공통 계약입니다.
    /// 이벤트 구독과 GUI 생명주기는 각 Trigger 구현이 소유합니다.
    /// </summary>
    internal interface IEditorMethodTriggerHandler
    {
        Type TriggerType { get; }

        bool TryValidateParameters(
            EditorMethodParameterMetadata[] parameters,
            out string reason);
    }
}
#endif
