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

        /* TODO(P1-01, editor-injection): 임의 CustomEditor와 Method Attribute가 공존하도록 내부 Inspector
         * 접근은 버전별 백엔드로 격리하고, 탐색 실패 시 Injection 기능만 비활성화합니다.
         * 가능한 기능은 PropertyDrawer와 Undo.postprocessModifications 등 공식 확장 지점을 우선합니다.
         */
        /* TODO(P1-02, test-matrix): 최소 지원 버전인 Unity 6000.3.15f1 이상의 패치 버전별로 검증합니다.
         */
        /* TODO(P0-02, lifecycle): Assembly Reload 및 Domain Reload 비활성화 환경에서 콜백 구독과
         * 백엔드 인스턴스가 중복되거나 남지 않도록 검증합니다.
         */
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
