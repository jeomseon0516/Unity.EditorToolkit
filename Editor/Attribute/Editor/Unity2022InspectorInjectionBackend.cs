#if UNITY_EDITOR && UNITY_2022_3_OR_NEWER && !UNITY_6000_0_OR_NEWER
using System.Collections.Generic;

namespace Jeomseon.Attribute.Editor
{
    /// <summary>
    /// Unity 2022.3 및 같은 InspectorElement 내부 구조를 사용하는 2023 계열 백엔드입니다.
    /// </summary>
    internal sealed class Unity2022InspectorInjectionBackend : InspectorElementInjectionBackendBase
    {
        private static readonly string[] MemberNames = { "editor", "m_Editor" };

        public override string Name => "Unity 2022.3-2023 InspectorElement";
        protected override IReadOnlyList<string> EditorMemberNames => MemberNames;
    }
}
#endif
