#if UNITY_EDITOR
using System;
using System.Reflection;

namespace Jeomseon.Attribute.Editor
{
    /// <summary>
    /// Trigger가 공통 호출기로 전달하는 대상, 메서드 및 인자입니다.
    /// </summary>
    internal readonly struct EditorMethodInvocation
    {
        public EditorMethodInvocation(
            UnityEngine.Object target,
            MethodInfo method,
            object[] arguments = null)
        {
            Target = target;
            Method = method ?? throw new ArgumentNullException(nameof(method));
            Arguments = arguments ?? Array.Empty<object>();
        }

        public UnityEngine.Object Target { get; }
        public MethodInfo Method { get; }
        public object[] Arguments { get; }
    }
}
#endif
