#if TOOLS
using System;
using System.Collections.Generic;
using System.Reflection;
using Godot;

namespace GodotNodeComponents.Editor;

public class CommonAttributeUtility
{
    private static Dictionary<Type, Dictionary<Type, HashSet<string>>> _cachedHasAttributes = [];

    private static HashSet<string> _GetAttributes(Type type, Type attributeType)
    {
        var result = new HashSet<string>();
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var field in fields)
        {
            var attrs = field.GetCustomAttributes(attributeType, true);
            if (attrs.Length > 0)
            {
                result.Add(field.Name);
            }
        }
        foreach (var prop in properties)
        {
            var attrs = prop.GetCustomAttributes(attributeType, true);
            if (attrs.Length > 0)
            {
                result.Add(prop.Name);
            }
        }

        var baseType = type.BaseType;
        if (baseType != null && baseType != typeof(object) && baseType != typeof(Node))
        {
            var baseAttributes = _GetAttributes(baseType, attributeType);
            foreach (var attr in baseAttributes)
            {
                result.Add(attr);
            }
        }

        return result;
    }

    public static bool HasAttribute<T>(GodotObject obj, string propertyName) where T : Attribute
    {
        if (obj == null) return false;
        if (string.IsNullOrEmpty(propertyName)) return false;
        var type = obj.GetType();
        var attributeType = typeof(T);
        if (!_cachedHasAttributes.ContainsKey(attributeType))
        {
            _cachedHasAttributes[attributeType] = [];
        }
        var attributeDic = _cachedHasAttributes[attributeType];
        if (attributeDic.TryGetValue(type, out var propsWithAttr))
        {
            return propsWithAttr.Contains(propertyName);
        }
        else
        {
            propsWithAttr = _GetAttributes(type, attributeType);
            attributeDic[type] = propsWithAttr;
            return propsWithAttr.Contains(propertyName);
        }
    }
}
#endif
