#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Jeomseon.Attribute.Editor
{
    /// <summary>
    /// 공통 Trigger 메서드 캐시를 필드 이름별 OnChangedValue 메서드 인덱스로 변환합니다.
    /// </summary>
    internal static class OnChangedValueMethodCache
    {
        private static readonly Dictionary<
            Type,
            IReadOnlyDictionary<
                string,
                EditorTriggeredMethod<OnChangedValueForMethodAttribute>[]>> Cache =
            new();

        public static IReadOnlyDictionary<
            string,
            EditorTriggeredMethod<OnChangedValueForMethodAttribute>[]> Get(
            Type targetType)
        {
            if (targetType == null)
                throw new ArgumentNullException(nameof(targetType));

            if (Cache.TryGetValue(
                    targetType,
                    out IReadOnlyDictionary<
                        string,
                        EditorTriggeredMethod<OnChangedValueForMethodAttribute>[]> methods))
            {
                return methods;
            }

            methods = BuildIndex(targetType);
            Cache.Add(targetType, methods);
            return methods;
        }

        private static IReadOnlyDictionary<
            string,
            EditorTriggeredMethod<OnChangedValueForMethodAttribute>[]> BuildIndex(
            Type targetType)
        {
            Dictionary<
                string,
                List<EditorTriggeredMethod<OnChangedValueForMethodAttribute>>> collecting =
                new(StringComparer.Ordinal);

            IEditorMethodTriggerHandler handler =
                EditorMethodTriggerHandlerRegistry
                    .Get<OnChangedValueForMethodAttribute>();

            foreach (EditorTriggeredMethod<OnChangedValueForMethodAttribute> item in
                     EditorTriggeredMethodCache<OnChangedValueForMethodAttribute>.Get(
                         targetType))
            {
                if (!handler.TryValidateParameters(
                        item.Parameters,
                        out string reason))
                {
                    /* TODO(P2, editor-method-arguments): 공통 매개변수 메타데이터는 준비되어
                     * 있습니다. OnChangedValueForMethod에서 매개변수를 지원할 때는 변경 대상,
                     * propertyPath, 이전 값과 새 값 중 어떤 정보를 전달할지 명시적인 규칙을 정의하고
                     * 이벤트 컨텍스트에서 object[]을 생성합니다. InspectorButton의 사용자 입력값과
                     * 달리 이 인자는 영구 직렬화하지 않고 동일한 EditorMethodInvocation 및
                     * EditorMethodInvoker 호출 경로를 사용합니다.
                     */
                    Debug.LogWarning(
                        $"[{nameof(OnChangedValueForMethodProcessor)}] " +
                        $"지원하지 않는 메서드입니다: " +
                        $"{item.Method.DeclaringType?.FullName}.{item.Method.Name}. " +
                        reason);
                    continue;
                }

                foreach (string fieldName in item.Trigger.FieldNames)
                {
                    if (string.IsNullOrWhiteSpace(fieldName))
                        continue;

                    AddMethod(collecting, fieldName, item);

                    // SerializedProperty에는 자동 구현 프로퍼티의 backing field 이름이 기록됩니다.
                    AddMethod(
                        collecting,
                        $"<{fieldName}>k__BackingField",
                        item);
                }
            }

            Dictionary<
                string,
                EditorTriggeredMethod<OnChangedValueForMethodAttribute>[]> result =
                new(collecting.Count, StringComparer.Ordinal);

            foreach (KeyValuePair<
                         string,
                         List<EditorTriggeredMethod<OnChangedValueForMethodAttribute>>>
                     pair in collecting)
            {
                result.Add(pair.Key, pair.Value.ToArray());
            }

            return result;
        }

        private static void AddMethod(
            IDictionary<
                string,
                List<EditorTriggeredMethod<OnChangedValueForMethodAttribute>>>
                methodsByField,
            string fieldName,
            EditorTriggeredMethod<OnChangedValueForMethodAttribute> method)
        {
            if (!methodsByField.TryGetValue(
                    fieldName,
                    out List<EditorTriggeredMethod<OnChangedValueForMethodAttribute>>
                        methods))
            {
                methods =
                    new List<EditorTriggeredMethod<OnChangedValueForMethodAttribute>>();
                methodsByField.Add(fieldName, methods);
            }

            if (!ContainsMethod(methods, method.Method))
                methods.Add(method);
        }

        private static bool ContainsMethod(
            IEnumerable<EditorTriggeredMethod<OnChangedValueForMethodAttribute>> methods,
            MethodInfo candidate)
        {
            foreach (EditorTriggeredMethod<OnChangedValueForMethodAttribute> method in
                     methods)
            {
                if (method.Method == candidate)
                    return true;
            }

            return false;
        }
    }
}
#endif
