#if UNITY_EDITOR
using System.Reflection;

namespace Jeomseon.Attribute.Editor
{
    /// <summary>
    /// InspectorButton을 그리거나 호출하는 데 필요한 메서드 메타데이터입니다.
    /// </summary>
    internal readonly struct InspectorButtonMethod
    {
        public InspectorButtonMethod(
            string label,
            EditorTriggeredMethod<InspectorButtonAttribute> metadata)
        {
            Label = label;
            Metadata = metadata;
        }

        public string Label { get; }
        public EditorTriggeredMethod<InspectorButtonAttribute> Metadata { get; }
        public MethodInfo Method => Metadata.Method;
        public EditorMethodParameterMetadata[] Parameters => Metadata.Parameters;
    }
}
#endif
