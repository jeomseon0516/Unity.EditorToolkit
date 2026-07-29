#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Jeomseon.Attribute.Editor
{
    /// <summary>
    /// 버전별 Inspector Injection 백엔드의 생명주기를 관리합니다.
    /// 내부 Unity API 변경이 발생하면 서비스가 아닌 해당 버전 백엔드만 수정합니다.
    /// </summary>
    [InitializeOnLoad]
    internal static class InspectorInjectionService
    {
        private static IInspectorInjectionBackend _backend;

        static InspectorInjectionService()
        {
            AssemblyReloadEvents.beforeAssemblyReload += Dispose;
            EditorApplication.delayCall += Initialize;
        }

        private static void Initialize()
        {
            Dispose();
            _backend = InspectorInjectionBackendFactory.Create();

            if (!_backend.IsSupported)
            {
                Debug.LogWarning(
                    $"[Jeomseon Inspector Injection] 현재 Unity 버전을 지원하지 않습니다. " +
                    $"선택된 백엔드: {_backend.Name}");
                return;
            }

            _backend.Start();
        }

        private static void Dispose()
        {
            _backend?.Dispose();
            _backend = null;
        }
    }
}
#endif
