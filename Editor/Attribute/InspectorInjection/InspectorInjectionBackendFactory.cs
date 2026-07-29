#if UNITY_EDITOR
namespace Jeomseon.Attribute.Editor
{
    internal static class InspectorInjectionBackendFactory
    {
        public static IInspectorInjectionBackend Create()
        {
#if UNITY_6000_0_OR_NEWER
            return new Unity6InspectorInjectionBackend();
#elif UNITY_2022_3_OR_NEWER
            return new Unity2022InspectorInjectionBackend();
#else
            return new UnsupportedInspectorInjectionBackend();
#endif
        }
    }
}
#endif
