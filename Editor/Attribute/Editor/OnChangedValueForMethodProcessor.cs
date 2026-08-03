#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

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
        private static readonly HashSet<InvocationKey> PendingInvocations = new();

        static OnChangedValueForMethodProcessor()
        {
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
                IReadOnlyDictionary<
                    string,
                    EditorTriggeredMethod<OnChangedValueForMethodAttribute>[]>
                    methodsByField =
                    OnChangedValueMethodCache.Get(target.GetType());
                string rootFieldName = GetRootFieldName(current.propertyPath);

                if (!methodsByField.TryGetValue(
                        rootFieldName,
                        out EditorTriggeredMethod<OnChangedValueForMethodAttribute>[]
                            methods))
                {
                    continue;
                }

                foreach (EditorTriggeredMethod<OnChangedValueForMethodAttribute> method in
                         methods)
                {
                    PendingInvocations.Add(
                        new InvocationKey(target, method.Method));
                }
            }

            if (PendingInvocations.Count > 0)
            {
                EditorApplication.delayCall -= InvokePendingMethods;
                EditorApplication.delayCall += InvokePendingMethods;
            }

            return modifications;
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

                EditorMethodInvoker.TryInvoke(
                    new EditorMethodInvocation(
                        invocation.Target,
                        invocation.Method));
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
