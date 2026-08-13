#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Jeomseon.Unity.EditorToolkit.Editor.Extensions
{
    using Attribute = Attribute;

    public static class SerializedPropertyExtensions
    {
        public static Type GetPropertyType(this SerializedProperty prop)
        {
            Object targetObject = prop?.serializedObject.targetObject;
            return !targetObject
                ? null
                : SerializedPropertyReflection.GetPropertyType(targetObject, prop.propertyPath);
        }

        public static Type GetParentType(this SerializedProperty prop)
        {
            // SerializedObject의 targetObject (루트 객체)를 가져옴
            Type rootType = prop.serializedObject.targetObject.GetType();
            Type targetType = rootType;

            // propertyPath를 '.' 기준으로 나누어 부모 필드를 추적
            string[] fieldNames = prop.propertyPath.Replace(".Array.data[", "[").Split('.');

            // 마지막 필드 이전까지 추적해서 부모 필드의 타입을 얻음
            for (int i = 0; i < fieldNames.Length - 1; i++)
            {
                if (fieldNames[i].Contains("["))
                {
                    // 배열 또는 리스트 타입 추적 (배열 요소의 타입을 찾음)
                    string fieldName = fieldNames[i][..fieldNames[i].IndexOf("[", StringComparison.Ordinal)];
                    FieldInfo field = targetType?.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                    // 배열이나 리스트라면 요소 타입을 추적함
                    if (field is not null)
                    {
                        targetType = field.FieldType.IsArray ? field.FieldType.GetElementType() : field.FieldType.GetGenericArguments()[0];
                    }
                }
                else
                {
                    // 일반 필드라면 해당 필드의 타입을 추적
                    if (targetType is not null)
                    { 
                        FieldInfo field = targetType.GetField(fieldNames[i], BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                        targetType = field?.FieldType;
                    }
                }
            }

            return targetType ?? rootType; // 부모 필드의 타입 반환
        }
        
        // SerializedProperty의 부모 PropertyPath를 얻는 메서드
        public static string GetParentPropertyPath(this SerializedProperty prop)
        {
            string path = prop.propertyPath;

            // 마지막 필드를 제외하고 부모 경로 반환
            int lastDotIndex = path.LastIndexOf('.');
            return lastDotIndex == -1 ? string.Empty : path[..lastDotIndex];

        }

        public static bool IsNestedAttribute<TAttributeType>(this SerializedProperty prop) where TAttributeType : Attribute
        {
            return SerializedPropertyReflection.HasAttribute<TAttributeType>(
                prop.serializedObject.targetObject,
                prop.propertyPath);
        }

        public static object GetPropertyValue(this SerializedProperty prop) => prop?.propertyType switch
        {
            null => null,
            SerializedPropertyType.Enum => prop.GetEnumData(),
            SerializedPropertyType.LayerMask =>
                prop.intValue,
            SerializedPropertyType.AnimationCurve =>
                prop.animationCurveValue,
            SerializedPropertyType.Gradient =>
                prop.gradientValue,
            SerializedPropertyType.ArraySize =>
                prop.arraySize,
            SerializedPropertyType.FixedBufferSize =>
                prop.fixedBufferSize,
            _ => prop.boxedValue
        };

        public static SerializedEnumData GetEnumData(this SerializedProperty prop)
        {
            if (prop?.propertyType != SerializedPropertyType.Enum)
                return null;

            int index = prop.enumValueIndex;
            string[] names = prop.enumNames;
            string[] displayNames = prop.enumDisplayNames;
            Type enumType = prop.GetPropertyType();
            object rawValue = GetEnumRawValue(prop, enumType);
            object value = enumType?.IsEnum == true
                ? Enum.ToObject(enumType, rawValue)
                : rawValue;

            return new SerializedEnumData(
                enumType,
                value,
                index,
                names != null && index >= 0 && index < names.Length ? names[index] : null,
                displayNames != null && index >= 0 && index < displayNames.Length ? displayNames[index] : null);
        }

        private static object GetEnumRawValue(SerializedProperty prop, Type enumType)
        {
            TypeCode typeCode = enumType?.IsEnum == true
                ? Type.GetTypeCode(Enum.GetUnderlyingType(enumType))
                : TypeCode.Int32;

            return typeCode switch
            {
                TypeCode.UInt32 => prop.uintValue,
                TypeCode.UInt64 => prop.ulongValue,
                TypeCode.Int64 => prop.longValue,
                _ => prop.intValue
            };
        }
    }
}
#endif
