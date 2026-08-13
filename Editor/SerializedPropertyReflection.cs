#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Jeomseon.Unity.EditorToolkit.Editor
{
    using Attribute = System.Attribute;

    /// <summary>
    /// Unity SerializedProperty 경로를 CLR 필드 정보로 변환합니다.
    /// </summary>
    public static class SerializedPropertyReflection
    {
        private const BindingFlags FieldFlags =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public static FieldInfo GetFieldInfo(object targetObject, string propertyPath)
        {
            return targetObject == null
                ? null
                : GetFieldInfo(targetObject.GetType(), propertyPath);
        }

        public static FieldInfo GetFieldInfo(Type rootType, string propertyPath)
        {
            if (rootType == null || string.IsNullOrEmpty(propertyPath))
            {
                return null;
            }

            Type currentType = rootType;
            FieldInfo fieldInfo = null;

            foreach (string part in propertyPath.Split('.'))
            {
                if (part == "Array" || part.StartsWith("data[", StringComparison.Ordinal))
                {
                    continue;
                }

                fieldInfo = GetFieldIncludingBaseTypes(currentType, part);
                if (fieldInfo == null)
                {
                    return null;
                }

                currentType = GetElementType(fieldInfo.FieldType);
            }

            return fieldInfo;
        }

        public static Type GetPropertyType(object targetObject, string propertyPath)
        {
            return GetFieldInfo(targetObject, propertyPath)?.FieldType;
        }

        public static IEnumerable<Attribute> GetAttributes(
            object targetObject,
            string propertyPath)
        {
            FieldInfo field = GetFieldInfo(targetObject, propertyPath);
            return field == null
                ? Array.Empty<Attribute>()
                : field.GetCustomAttributes<Attribute>();
        }

        public static bool HasAttribute<TAttribute>(
            object targetObject,
            string propertyPath)
            where TAttribute : Attribute
        {
            return GetFieldInfo(targetObject, propertyPath)?
                .GetCustomAttribute<TAttribute>() != null;
        }

        public static string GetBackingFieldName(string propertyName)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            return $"<{propertyName}>k__BackingField";
        }

        private static FieldInfo GetFieldIncludingBaseTypes(Type type, string fieldName)
        {
            for (Type currentType = type; currentType != null; currentType = currentType.BaseType)
            {
                FieldInfo field = currentType.GetField(fieldName, FieldFlags);
                if (field != null)
                {
                    return field;
                }
            }

            return null;
        }

        private static Type GetElementType(Type type)
        {
            if (type.IsArray)
            {
                return type.GetElementType();
            }

            if (type.IsGenericType)
            {
                Type genericType = type.GetGenericTypeDefinition();
                if (genericType == typeof(List<>) ||
                    genericType == typeof(IList<>) ||
                    genericType == typeof(IEnumerable<>))
                {
                    return type.GetGenericArguments()[0];
                }
            }

            return type;
        }
    }
}
#endif
