#if UNITY_EDITOR
using System;
using System.Reflection;

namespace Jeomseon.Attribute.Editor
{
    /// <summary>
    /// 모든 Editor Method Trigger가 공유하는 CLR 매개변수 제약을 검사합니다.
    /// </summary>
    internal static class EditorMethodParameterValidator
    {
        public static EditorMethodParameterMetadata[] CreateMetadata(
            ParameterInfo[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
                return Array.Empty<EditorMethodParameterMetadata>();

            EditorMethodParameterMetadata[] result =
                new EditorMethodParameterMetadata[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo parameter = parameters[i];
                bool supported = TryValidate(parameter, out string reason);
                result[i] = new EditorMethodParameterMetadata(
                    parameter,
                    supported,
                    reason);
            }

            return result;
        }

        public static bool TryValidate(
            ParameterInfo parameter,
            out string reason)
        {
            if (parameter == null)
            {
                reason = "매개변수 메타데이터가 없습니다.";
                return false;
            }

            Type type = parameter.ParameterType;
            if (type.IsByRef || parameter.IsOut || parameter.IsIn)
            {
                reason = "ref, out, in 매개변수는 지원하지 않습니다.";
                return false;
            }

            if (type.IsPointer)
            {
                reason = "포인터 매개변수는 지원하지 않습니다.";
                return false;
            }

            if (type.ContainsGenericParameters)
            {
                reason = "열린 제네릭 매개변수는 지원하지 않습니다.";
                return false;
            }

            if (type.IsByRefLike)
            {
                reason = "ref struct 매개변수는 지원하지 않습니다.";
                return false;
            }

            reason = string.Empty;
            return true;
        }
    }
}
#endif
