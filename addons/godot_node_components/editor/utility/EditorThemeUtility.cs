#if TOOLS
using Godot;

namespace GodotNodeComponents.Editor;

internal static class EditorThemeUtility
{
    public static Control GetControl()
    {
        return EditorInterface.Singleton.GetBaseControl();
    }
    public static Texture2D GetEditorIcon(StringName name)
    {
        return GetIcon(name, "EditorIcons");
    }
    public static Texture2D GetIcon(StringName name, StringName type)
    {
        return GetControl().GetThemeIcon(name, type);
    }
    public static Font GetFont(StringName name, StringName type)
    {
        return GetControl().GetThemeFont(name, type);
    }
    public static int GetFontSize(StringName name, StringName type)
    {
        return GetControl().GetThemeFontSize(name, type);
    }
    public static Color GetColor(StringName name, StringName type)
    {
        return GetControl().GetThemeColor(name, type);
    }
    public static StyleBox GetStylebox(StringName name, StringName type)
    {
        return GetControl().GetThemeStylebox(name, type);
    }
}
#endif
