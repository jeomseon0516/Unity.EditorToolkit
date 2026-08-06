#if UNITY_EDITOR
using System;

namespace Jeomseon.Editor.Extensions
{
    /// <summary>
    /// Provides the value and metadata of an enum SerializedProperty.
    /// </summary>
    public sealed class SerializedEnumData
    {
        public Type EnumType { get; }
        public object Value { get; }
        public int Index { get; }
        public string Name { get; }
        public string DisplayName { get; }

        public SerializedEnumData(
            Type enumType,
            object value,
            int index,
            string name,
            string displayName)
        {
            EnumType = enumType;
            Value = value;
            Index = index;
            Name = name;
            DisplayName = displayName;
        }
    }
}
#endif
