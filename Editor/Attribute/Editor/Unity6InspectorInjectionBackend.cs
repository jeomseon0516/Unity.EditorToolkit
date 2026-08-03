#if UNITY_EDITOR
using System.Collections.Generic;

namespace Jeomseon.Attribute.Editor
{
    /// <summary>
    /// Unity 6 InspectorElement 내부 구조용 백엔드입니다.
    /// 패치 버전에서 멤버 이름이 바뀌면 이 후보 목록과 검증 테스트만 갱신합니다.
    /// </summary>
    internal sealed class Unity6InspectorInjectionBackend : InspectorElementInjectionBackendBase
    {
        private static readonly string[] MemberNames =
            { "editor", "m_Editor", "m_InspectorEditor" };

        public override string Name => "Unity 6 InspectorElement";
        protected override IReadOnlyList<string> EditorMemberNames => MemberNames;
    }
}
#endif
