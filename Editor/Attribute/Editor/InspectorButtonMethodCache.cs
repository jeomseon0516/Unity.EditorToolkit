#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Jeomseon.Attribute.Editor
{
    /// <summary>
    /// 대상 타입별 InspectorButton 메서드 메타데이터를 수집하고 캐시합니다.
    /// </summary>
    internal static class InspectorButtonMethodCache
    {
        private static readonly Dictionary<Type, InspectorButtonMethod[]> Cache = new();

        public static InspectorButtonMethod[] Get(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (Cache.TryGetValue(type, out InspectorButtonMethod[] methods))
                return methods;

            methods = Collect(type);
            Cache.Add(type, methods);
            return methods;
        }

        private static InspectorButtonMethod[] Collect(Type type)
        {
            List<InspectorButtonMethod> result = new();
            IEditorMethodTriggerHandler handler =
                EditorMethodTriggerHandlerRegistry.Get<InspectorButtonAttribute>();

            foreach (EditorTriggeredMethod<InspectorButtonAttribute> item in
                     EditorTriggeredMethodCache<InspectorButtonAttribute>.Get(type))
            {
                if (!handler.TryValidateParameters(
                        item.Parameters,
                        out string reason))
                {
                    /* TODO(P2, inspector-method-arguments): 공통 EditorTriggeredMethod의
                     * 매개변수 메타데이터를 사용해 Inspector 인자 입력 GUI를 추가합니다.
                     * - 저장된 입력값이 없으면 선택적 매개변수의 기본값, 그 외에는 default(T)로 초기화합니다.
                     * - 입력 상태는 Inspector 세션 저장소부터 구현하고, 영구 저장이 필요하면
                     *   ProjectSettings 또는 대상 객체가 제공하는 직렬화 저장소를 별도 정책으로 추가합니다.
                     * - primitive, string, enum, UnityEngine.Object 및 Unity 기본 값 타입부터 지원합니다.
                     * - ref/out/in, 포인터, 열린 제네릭 및 임의의 복합 타입은 초기 지원에서 제외합니다.
                     * - 다중 선택에서는 대상별 값을 보관하고 mixed value 표시와 일괄 변경을 지원합니다.
                     * - 완성된 object[]은 EditorMethodInvocation과 EditorMethodInvoker로 호출합니다.
                     */
                    Debug.LogWarning(
                        $"[{nameof(InspectorButtonGUI)}] 지원하지 않는 메서드입니다: " +
                        $"{item.Method.DeclaringType?.FullName}.{item.Method.Name}. " +
                        reason);
                    continue;
                }

                string label = string.IsNullOrWhiteSpace(item.Trigger.ButtonName)
                    ? ObjectNames.NicifyVariableName(item.Method.Name)
                    : item.Trigger.ButtonName;

                result.Add(new InspectorButtonMethod(label, item));
            }

            return result.Count == 0
                ? Array.Empty<InspectorButtonMethod>()
                : result.ToArray();
        }
    }
}
#endif
