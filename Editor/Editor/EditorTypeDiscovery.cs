#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Jeomseon.Editor
{
    /// <summary>
    /// Unity Editor가 색인한 TypeCache를 사용해 타입과 현재 로드된 Unity 객체를 탐색합니다.
    /// </summary>
    public static class EditorTypeDiscovery
    {
        public static IEnumerable<Type> GetConcreteTypesDerivedFrom(Type baseType)
        {
            if (baseType == null)
            {
                throw new ArgumentNullException(nameof(baseType));
            }

            return TypeCache.GetTypesDerivedFrom(baseType)
                .Where(type =>
                    type != baseType &&
                    !type.IsInterface &&
                    !type.IsAbstract &&
                    !type.ContainsGenericParameters);
        }

        public static IEnumerable<Type> GetConcreteTypesDerivedFrom<T>()
        {
            return GetConcreteTypesDerivedFrom(typeof(T));
        }

        public static IEnumerable<Type> GetTypesDirectlyDerivedFromGeneric(Type genericTypeDefinition)
        {
            ValidateGenericTypeDefinition(genericTypeDefinition);

            return TypeCache.GetTypesDerivedFrom(genericTypeDefinition)
                .Where(type =>
                    type.BaseType is { IsGenericType: true } baseType &&
                    baseType.GetGenericTypeDefinition() == genericTypeDefinition);
        }

        public static IEnumerable<T> FindLoadedObjectsDirectlyDerivedFromGeneric<T>(
            Type genericTypeDefinition)
            where T : class
        {
            foreach (Type type in GetTypesDirectlyDerivedFromGeneric(genericTypeDefinition))
            {
                foreach (T instance in Resources.FindObjectsOfTypeAll(type).OfType<T>())
                {
                    yield return instance;
                }
            }
        }

        private static void ValidateGenericTypeDefinition(Type genericTypeDefinition)
        {
            if (genericTypeDefinition == null)
            {
                throw new ArgumentNullException(nameof(genericTypeDefinition));
            }

            if (!genericTypeDefinition.IsGenericTypeDefinition)
            {
                throw new ArgumentException(
                    "The type must be a generic type definition.",
                    nameof(genericTypeDefinition));
            }
        }
    }
}
#endif
