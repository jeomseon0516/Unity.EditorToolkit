#if UNITY_EDITOR
using System;
using System.Reflection;

namespace Jeomseon.Attribute.Editor
{
    /// <summary>
    /// Trigger Handler가 인자 입력 또는 이벤트 인자 바인딩에 사용하는 매개변수 정보입니다.
    /// </summary>
    internal readonly struct EditorMethodParameterMetadata
    {
        public EditorMethodParameterMetadata(
            ParameterInfo parameter,
            bool isSupported,
            string unsupportedReason)
        {
            Parameter = parameter ??
                        throw new ArgumentNullException(nameof(parameter));
            IsSupported = isSupported;
            UnsupportedReason = unsupportedReason ?? string.Empty;
        }

        public ParameterInfo Parameter { get; }
        public string Name => Parameter.Name ?? string.Empty;
        public Type ParameterType => Parameter.ParameterType;
        public bool HasDefaultValue => Parameter.HasDefaultValue;
        public object DefaultValue => Parameter.DefaultValue;
        public bool IsSupported { get; }
        public string UnsupportedReason { get; }
    }
}
#endif
