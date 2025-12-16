#if TOOLS
using Godot;

namespace GodotNodeComponents.Editor;

public partial class CommonAttributeInspector : EditorInspectorPlugin
{
    public override bool _CanHandle(GodotObject @object)
    {
        return true;
    }
    public override bool _ParseProperty(GodotObject @object, Variant.Type type,
        string name, PropertyHint hintType, string hintString,
        PropertyUsageFlags usageFlags, bool wide)
    {
        if (CommonAttributeUtility.HasAttribute<HideInInspectorAttribute>(@object, name))
        {
            return true;
        }
        return false;
    }
}
#endif
