#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Jeomseon.Unity.EditorToolkit.Editor
{
    using Attribute = System.Attribute;

    /// <summary>
    /// SerializeField 또는 SerializeReference로 연결된 일반 C# 객체 그래프를 순회합니다.
    /// UnityEngine.Object 생명주기를 가진 객체는 그래프 경계로 취급합니다.
    /// </summary>
    public static class SerializedObjectGraphTraversal
    {
        private const BindingFlags MemberFlags =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public static IEnumerable<MethodInfo> GetMethodsWithAttribute<TAttribute>(object target)
            where TAttribute : Attribute
        {
            if (target == null)
            {
                yield break;
            }

            for (Type currentType = target.GetType();
                 ShouldSearchMethods(currentType);
                 currentType = currentType.BaseType)
            {
                foreach (MethodInfo method in currentType.GetMethods(
                             BindingFlags.DeclaredOnly |
                             BindingFlags.Instance |
                             BindingFlags.Static |
                             BindingFlags.Public |
                             BindingFlags.NonPublic))
                {
                    if (method.GetCustomAttribute<TAttribute>() != null)
                    {
                        yield return method;
                    }
                }
            }
        }

        public static IEnumerable<Action> GetActionsWithAttribute<TAttribute>(
            object root,
            HashSet<object> visited = null)
            where TAttribute : Attribute
        {
            foreach (object target in Traverse(root, visited))
            {
                foreach (MethodInfo method in GetMethodsWithAttribute<TAttribute>(target))
                {
                    if (method.ReturnType != typeof(void) ||
                        method.GetParameters().Length != 0)
                    {
                        continue;
                    }

                    yield return method.IsStatic
                        ? (Action)Delegate.CreateDelegate(typeof(Action), method)
                        : (Action)Delegate.CreateDelegate(typeof(Action), target, method);
                }
            }
        }

        public static IEnumerable<ICollection> GetCollections(
            object root,
            HashSet<object> visited = null)
        {
            foreach (object target in Traverse(root, visited))
            {
                if (target is ICollection collection)
                {
                    yield return collection;
                }
            }
        }

        public static IEnumerable<object> Traverse(
            object root,
            HashSet<object> visited = null)
        {
            visited ??= new HashSet<object>(ReferenceEqualityComparer.Instance);
            foreach (object target in TraverseCore(root, visited, true))
            {
                yield return target;
            }
        }

        private static IEnumerable<object> TraverseCore(
            object target,
            HashSet<object> visited,
            bool isRoot = false)
        {
            if (target == null ||
                (!isRoot && target is UnityEngine.Object) ||
                !visited.Add(target))
            {
                yield break;
            }

            yield return target;

            if (target is IEnumerable enumerable and not string)
            {
                foreach (object item in enumerable)
                {
                    foreach (object nested in TraverseCore(item, visited))
                    {
                        yield return nested;
                    }
                }
            }

            Type type = target.GetType();
            foreach (FieldInfo field in type.GetFields(MemberFlags))
            {
                if (!ShouldTraverse(field, field.FieldType))
                {
                    continue;
                }

                foreach (object nested in TraverseCore(field.GetValue(target), visited))
                {
                    yield return nested;
                }
            }

            foreach (PropertyInfo property in type.GetProperties(MemberFlags))
            {
                if (!property.CanRead ||
                    property.GetIndexParameters().Length != 0 ||
                    !ShouldTraverse(property, property.PropertyType))
                {
                    continue;
                }

                object value;
                try
                {
                    value = property.GetValue(target);
                }
                catch (TargetInvocationException)
                {
                    continue;
                }

                foreach (object nested in TraverseCore(value, visited))
                {
                    yield return nested;
                }
            }
        }

        private static bool ShouldSearchMethods(Type type)
        {
            return type != null &&
                type != typeof(MonoBehaviour) &&
                type != typeof(Component) &&
                type != typeof(ScriptableObject) &&
                type != typeof(GameObject);
        }

        private static bool ShouldTraverse(MemberInfo member, Type memberType)
        {
            return !member.IsDefined(typeof(ObsoleteAttribute), true) &&
                (member.IsDefined(typeof(SerializeField), true) ||
                 member.IsDefined(typeof(SerializeReference), true)) &&
                (memberType.IsClass || memberType.IsInterface) &&
                !typeof(UnityEngine.Object).IsAssignableFrom(memberType);
        }

        private sealed class ReferenceEqualityComparer : IEqualityComparer<object>
        {
            public static readonly ReferenceEqualityComparer Instance = new();

            public new bool Equals(object x, object y)
            {
                return ReferenceEquals(x, y);
            }

            public int GetHashCode(object obj)
            {
                return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
            }
        }
    }
}
#endif
