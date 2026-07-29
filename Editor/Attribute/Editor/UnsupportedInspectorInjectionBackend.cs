#if UNITY_EDITOR && !UNITY_2022_3_OR_NEWER
namespace Jeomseon.Attribute.Editor
{
    internal sealed class UnsupportedInspectorInjectionBackend : IInspectorInjectionBackend
    {
        public string Name => "Unsupported Unity Inspector";
        public bool IsSupported => false;
        public void Start() { }
        public void Dispose() { }
    }
}
#endif
