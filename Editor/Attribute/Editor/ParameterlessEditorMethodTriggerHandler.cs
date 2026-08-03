#if UNITY_EDITOR
using System;

namespace Jeomseon.Attribute.Editor
{
    /// <summary>
    /// 아직 인자를 제공하지 않는 Trigger의 공통 매개변수 정책입니다.
    /// </summary>
    internal abstract class ParameterlessEditorMethodTriggerHandler<TTrigger>
        : IEditorMethodTriggerHandler
        where TTrigger : EditorMethodTriggerAttribute
    {
        public Type TriggerType => typeof(TTrigger);

        public bool TryValidateParameters(
            EditorMethodParameterMetadata[] parameters,
            out string reason)
        {
            if (parameters == null || parameters.Length == 0)
            {
                reason = string.Empty;
                return true;
            }

            foreach (EditorMethodParameterMetadata parameter in parameters)
            {
                if (!parameter.IsSupported)
                {
                    reason =
                        $"{parameter.Name}: {parameter.UnsupportedReason}";
                    return false;
                }
            }

            reason = "이 Trigger는 아직 매개변수 입력을 지원하지 않습니다.";
            return false;
        }
    }
}
#endif
