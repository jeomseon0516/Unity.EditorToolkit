#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Jeomseon.Attribute.Editor
{
    /// <summary>
    /// Editor Method Trigger가 공유하는 대상 검증, 메서드 호출 및 변경 기록을 담당합니다.
    /// </summary>
    internal static class EditorMethodInvoker
    {
        public static bool CanInvoke(
            EditorMethodInvocation invocation)
        {
            return invocation.Target != null &&
                   invocation.Method != null &&
                   invocation.Method.DeclaringType != null &&
                   invocation.Method.DeclaringType.IsInstanceOfType(
                       invocation.Target);
        }

        public static bool TryInvoke(EditorMethodInvocation invocation)
        {
            if (!CanInvoke(invocation))
                return false;

            try
            {
                invocation.Method.Invoke(
                    invocation.Target,
                    invocation.Arguments);
                EditorUtility.SetDirty(invocation.Target);
                PrefabUtility.RecordPrefabInstancePropertyModifications(
                    invocation.Target);
                return true;
            }
            catch (TargetInvocationException exception)
            {
                Debug.LogException(
                    exception.InnerException ?? exception,
                    invocation.Target);
                return false;
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, invocation.Target);
                return false;
            }
        }
    }
}
#endif
