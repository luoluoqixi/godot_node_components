
using System.Reflection;
using System.Text.Json;
using Godot;

namespace GodotNodeComponents;

public static class NodeComponentsUtility
{
    public static void DefaultSaveComponents(this Node node, string[] data)
    {
        node.SetMeta(ComponentsConfig.ComponentsMetaKey, data);
    }

    public static string[] DefaultLoadComponents(this Node node)
    {
        if (!node.HasMeta(ComponentsConfig.ComponentsMetaKey))
            return null;
        var data = node.GetMeta(ComponentsConfig.ComponentsMetaKey);
        if (data.VariantType == Variant.Type.Nil) return null;
        if (data.VariantType != Variant.Type.PackedStringArray)
        {
            GD.PrintErr("Components data is corrupted.");
            return null;
        }
        return data.As<string[]>();
    }

    public static void ClearJsonCache()
    {
        // https://github.com/godotengine/godot/issues/78513
        var assembly = typeof(JsonSerializerOptions).Assembly;
        var updateHandlerType = assembly.GetType("System.Text.Json.JsonSerializerOptionsUpdateHandler");
        var clearCacheMethod = updateHandlerType?.GetMethod("ClearCache", BindingFlags.Static | BindingFlags.Public);
        clearCacheMethod?.Invoke(null, new object[] { null });
    }
}
