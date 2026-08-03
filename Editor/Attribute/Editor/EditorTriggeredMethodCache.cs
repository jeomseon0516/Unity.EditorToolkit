#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Jeomseon.Attribute.Editor
{
    /// <summary>
    /// Unity TypeCache를 사용해 Trigger 메서드를 찾고 대상 타입의 상속 계층별로 캐시합니다.
    /// 메서드별 ParameterInfo도 함께 보관하여 향후 Inspector 인자 입력 기능에서 재사용합니다.
    /// </summary>
    internal static class EditorTriggeredMethodCache<TTrigger>
        where TTrigger : EditorMethodTriggerAttribute
    {
        private static readonly Dictionary<
            Type,
            EditorTriggeredMethod<TTrigger>[]> DeclaredMethodsByType =
            BuildDeclaredMethodsIndex();

        private static readonly Dictionary<
            Type,
            EditorTriggeredMethod<TTrigger>[]> MethodsByTargetType = new();

        public static EditorTriggeredMethod<TTrigger>[] Get(Type targetType)
        {
            if (targetType == null)
                throw new ArgumentNullException(nameof(targetType));

            if (MethodsByTargetType.TryGetValue(
                    targetType,
                    out EditorTriggeredMethod<TTrigger>[] methods))
            {
                return methods;
            }

            methods = BuildTargetMethods(targetType);
            MethodsByTargetType.Add(targetType, methods);
            return methods;
        }

        private static Dictionary<
            Type,
            EditorTriggeredMethod<TTrigger>[]> BuildDeclaredMethodsIndex()
        {
            Dictionary<Type, List<EditorTriggeredMethod<TTrigger>>> collecting =
                new();

            foreach (MethodInfo method in
                     TypeCache.GetMethodsWithAttribute<TTrigger>())
            {
                Type declaringType = method.DeclaringType;
                if (declaringType == null ||
                    method.IsStatic ||
                    method.IsAbstract ||
                    method.IsGenericMethodDefinition ||
                    method.IsSpecialName)
                {
                    continue;
                }

                TTrigger trigger =
                    method.GetCustomAttribute<TTrigger>(inherit: false);
                if (trigger == null)
                    continue;

                if (!collecting.TryGetValue(
                        declaringType,
                        out List<EditorTriggeredMethod<TTrigger>> methods))
                {
                    methods = new List<EditorTriggeredMethod<TTrigger>>();
                    collecting.Add(declaringType, methods);
                }

                methods.Add(new EditorTriggeredMethod<TTrigger>(
                    method,
                    trigger,
                    EditorMethodParameterValidator.CreateMetadata(
                        method.GetParameters())));
            }

            Dictionary<Type, EditorTriggeredMethod<TTrigger>[]> result = new();
            foreach (KeyValuePair<
                         Type,
                         List<EditorTriggeredMethod<TTrigger>>> pair in collecting)
            {
                result.Add(pair.Key, pair.Value.ToArray());
            }

            return result;
        }

        private static EditorTriggeredMethod<TTrigger>[] BuildTargetMethods(
            Type targetType)
        {
            List<EditorTriggeredMethod<TTrigger>> result = new();
            HashSet<MethodInfo> virtualSlots = new();

            for (Type current = targetType;
                 current != null && IsInspectableUserType(current);
                 current = current.BaseType)
            {
                if (!DeclaredMethodsByType.TryGetValue(
                        current,
                        out EditorTriggeredMethod<TTrigger>[] methods))
                {
                    continue;
                }

                foreach (EditorTriggeredMethod<TTrigger> item in methods)
                {
                    MethodInfo method = item.Method;
                    if (method.IsVirtual &&
                        !virtualSlots.Add(method.GetBaseDefinition()))
                    {
                        continue;
                    }

                    result.Add(item);
                }
            }

            return result.Count == 0
                ? Array.Empty<EditorTriggeredMethod<TTrigger>>()
                : result.ToArray();
        }

        private static bool IsInspectableUserType(Type type)
        {
            return type != typeof(MonoBehaviour) &&
                   type != typeof(ScriptableObject) &&
                   type != typeof(UnityEngine.Object) &&
                   type != typeof(object);
        }
    }
}
#endif
