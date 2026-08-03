#if UNITY_EDITOR
using System;
using System.Reflection;

namespace Jeomseon.Attribute.Editor
{
    /// <summary>
    /// Editor Method Trigger 기능이 공유하는 메서드와 매개변수 메타데이터입니다.
    /// </summary>
    internal readonly struct EditorTriggeredMethod<TTrigger>
        where TTrigger : EditorMethodTriggerAttribute
    {
        public EditorTriggeredMethod(
            MethodInfo method,
            TTrigger trigger,
            EditorMethodParameterMetadata[] parameters)
        {
            Method = method ?? throw new ArgumentNullException(nameof(method));
            Trigger = trigger ?? throw new ArgumentNullException(nameof(trigger));
            Parameters = parameters ?? Array.Empty<EditorMethodParameterMetadata>();
        }

        public MethodInfo Method { get; }
        public TTrigger Trigger { get; }
        public EditorMethodParameterMetadata[] Parameters { get; }
        public bool HasParameters => Parameters.Length != 0;
    }
}
#endif
