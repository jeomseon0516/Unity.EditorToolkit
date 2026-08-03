#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;

namespace Jeomseon.Attribute.Editor
{
    /// <summary>
    /// EditorMethodTriggerAttribute 타입과 처리 정책을 연결합니다.
    /// </summary>
    internal static class EditorMethodTriggerHandlerRegistry
    {
        private static readonly Dictionary<Type, IEditorMethodTriggerHandler> Handlers =
            BuildHandlers();

        public static IEditorMethodTriggerHandler Get<TTrigger>()
            where TTrigger : EditorMethodTriggerAttribute
        {
            Type triggerType = typeof(TTrigger);
            if (Handlers.TryGetValue(
                    triggerType,
                    out IEditorMethodTriggerHandler handler))
            {
                return handler;
            }

            throw new InvalidOperationException(
                $"등록된 Editor Method Trigger Handler가 없습니다: " +
                $"{triggerType.FullName}");
        }

        private static Dictionary<Type, IEditorMethodTriggerHandler> BuildHandlers()
        {
            Dictionary<Type, IEditorMethodTriggerHandler> result = new();

            foreach (Type type in
                     TypeCache.GetTypesDerivedFrom<IEditorMethodTriggerHandler>())
            {
                if (type.IsAbstract ||
                    type.IsInterface ||
                    type.ContainsGenericParameters)
                {
                    continue;
                }

                if (Activator.CreateInstance(type) is not
                    IEditorMethodTriggerHandler handler)
                {
                    continue;
                }

                if (result.ContainsKey(handler.TriggerType))
                {
                    throw new InvalidOperationException(
                        $"중복된 Editor Method Trigger Handler입니다: " +
                        $"{handler.TriggerType.FullName}");
                }

                result.Add(handler.TriggerType, handler);
            }

            return result;
        }
    }
}
#endif
