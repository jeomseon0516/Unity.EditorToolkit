#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Jeomseon.Attribute.Editor
{
    /// <summary>
    /// 지정한 직렬화 필드의 에디터 변경을 감지하고 연결된 메서드를 호출합니다.
    /// 인스펙터 렌더링 위치나 Repaint 주기에 의존하지 않고 공식 Undo 변경 알림을 사용합니다.
    /// 따라서 다른 CustomEditor가 정의되어 있어도 SerializedObject/SerializedProperty 또는
    /// Undo.RecordObject를 통해 기록한 변경이라면 동일하게 처리됩니다.
    /// </summary>
    [InitializeOnLoad]
    internal static class OnChangedValueForMethodProcessor
    {
        private static readonly Dictionary<Type, IReadOnlyDictionary<string, MethodInfo[]>> Cache = new();
        private static readonly HashSet<InvocationKey> PendingInvocations = new();

        static OnChangedValueForMethodProcessor()
        {
            // TODO(editor-contract): 필드를 직접 대입하고 Undo도 기록하지 않는 CustomEditor의 변경은
            // Unity 공식 API로 관찰할 수 없습니다. 이를 감지하기 위해 InspectorWindow 내부 구조나
            // 지속적인 리플렉션 폴링을 다시 도입하지 말고 해당 CustomEditor가 SerializedObject 또는
            // Undo.RecordObject를 사용하도록 수정해야 합니다.
            Undo.postprocessModifications += OnPostprocessModifications;
        }

        private static UndoPropertyModification[] OnPostprocessModifications(
            UndoPropertyModification[] modifications)
        {
            foreach (UndoPropertyModification modification in modifications)
            {
                PropertyModification current = modification.currentValue;
                if (current?.target == null || string.IsNullOrEmpty(current.propertyPath))
                    continue;

                UnityEngine.Object target = current.target;
                IReadOnlyDictionary<string, MethodInfo[]> methodsByField = GetMethods(target.GetType());
                string rootFieldName = GetRootFieldName(current.propertyPath);

                if (!methodsByField.TryGetValue(rootFieldName, out MethodInfo[] methods))
                    continue;

                foreach (MethodInfo method in methods)
                    PendingInvocations.Add(new InvocationKey(target, method));
            }

            if (PendingInvocations.Count > 0)
            {
                EditorApplication.delayCall -= InvokePendingMethods;
                EditorApplication.delayCall += InvokePendingMethods;
            }

            return modifications;
        }

        private static IReadOnlyDictionary<string, MethodInfo[]> GetMethods(Type type)
        {
            if (Cache.TryGetValue(type, out IReadOnlyDictionary<string, MethodInfo[]> cached))
                return cached;

            const BindingFlags Flags =
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            Dictionary<string, List<MethodInfo>> collecting = new(StringComparer.Ordinal);
            foreach (MethodInfo method in type.GetMethods(Flags))
            {
                OnChangedValueForMethodAttribute attribute =
                    method.GetCustomAttribute<OnChangedValueForMethodAttribute>();

                if (attribute == null)
                    continue;

                if (method.GetParameters().Length != 0)
                {
                    Debug.LogWarning(
                        $"[{nameof(OnChangedValueForMethodProcessor)}] 매개변수가 있는 메서드는 지원하지 않습니다: " +
                        $"{type.FullName}.{method.Name}");
                    continue;
                }

                foreach (string fieldName in attribute.FieldNames)
                {
                    if (string.IsNullOrWhiteSpace(fieldName))
                        continue;

                    AddMethod(collecting, fieldName, method);

                    // 자동 구현 프로퍼티 이름을 지정한 기존 사용 방식도 지원합니다.
                    // SerializedProperty의 실제 경로에는 backing field 이름이 기록됩니다.
                    AddMethod(collecting, $"<{fieldName}>k__BackingField", method);
                }
            }

            Dictionary<string, MethodInfo[]> result = new(StringComparer.Ordinal);
            foreach (KeyValuePair<string, List<MethodInfo>> pair in collecting)
                result.Add(pair.Key, pair.Value.ToArray());

            Cache[type] = result;
            return result;
        }

        private static void AddMethod(
            IDictionary<string, List<MethodInfo>> methodsByField,
            string fieldName,
            MethodInfo method)
        {
            if (!methodsByField.TryGetValue(fieldName, out List<MethodInfo> methods))
            {
                methods = new List<MethodInfo>();
                methodsByField.Add(fieldName, methods);
            }

            if (!methods.Contains(method))
                methods.Add(method);
        }

        private static string GetRootFieldName(string propertyPath)
        {
            int separatorIndex = propertyPath.IndexOf('.');
            return separatorIndex < 0 ? propertyPath : propertyPath[..separatorIndex];
        }

        private static void InvokePendingMethods()
        {
            if (PendingInvocations.Count == 0)
                return;

            InvocationKey[] invocations = new InvocationKey[PendingInvocations.Count];
            PendingInvocations.CopyTo(invocations);
            PendingInvocations.Clear();

            foreach (InvocationKey invocation in invocations)
            {
                if (invocation.Target == null)
                    continue;

                try
                {
                    invocation.Method.Invoke(invocation.Target, null);
                    EditorUtility.SetDirty(invocation.Target);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(invocation.Target);
                }
                catch (TargetInvocationException exception)
                {
                    Debug.LogException(exception.InnerException ?? exception, invocation.Target);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception, invocation.Target);
                }
            }
        }

        private readonly struct InvocationKey : IEquatable<InvocationKey>
        {
            public InvocationKey(UnityEngine.Object target, MethodInfo method)
            {
                Target = target;
                Method = method;
            }

            public UnityEngine.Object Target { get; }
            public MethodInfo Method { get; }

            public bool Equals(InvocationKey other)
            {
                return Target == other.Target && Method == other.Method;
            }

            public override bool Equals(object obj)
            {
                return obj is InvocationKey other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Target != null ? Target.GetInstanceID() : 0) * 397) ^
                           (Method != null ? Method.GetHashCode() : 0);
                }
            }
        }
    }
}
#endif
