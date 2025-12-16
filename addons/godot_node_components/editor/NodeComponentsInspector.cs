#if TOOLS
using System;
using System.Linq;
using Godot;

namespace GodotNodeComponents.Editor;

public partial class NodeComponentsInspector : EditorInspectorPlugin
{
    private Type[] _componentTypes;
    private Type[] ComponentTypes
    {
        get
        {
            if (_componentTypes == null)
            {
                _componentTypes =
                [
                    typeof(NodeComponents),
                ];
            }
            return _componentTypes;
        }
    }
    private string[] _componentTypeNames;
    private string[] ComponentTypeNames
    {
        get
        {
            if (_componentTypeNames == null)
            {
                _componentTypeNames = ComponentTypes
                    .Select(t => t.Name)
                    .ToArray();
            }
            return _componentTypeNames;
        }
    }

    private bool _inTargetCategory = false;
    private bool _uiDrawn = false;

    public override bool _CanHandle(GodotObject @object)
    {
        var types = ComponentTypes;
        var objType = @object.GetType();
        return types.Any(t => objType == t || objType.IsSubclassOf(t));
    }

    private void _DrawNodeComponentsUI()
    {
        var addComponent = new Button();
        addComponent.SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter;
        addComponent.Text = "Add Component";
        addComponent.Icon = _GetEditorIcon("Add");
        addComponent.Pressed += _OnAddComponentPressed;

        AddCustomControl(addComponent);
    }

    public override void _ParseCategory(GodotObject @object, string category)
    {
        base._ParseCategory(@object, category);
        var typeNames = ComponentTypeNames;

        _inTargetCategory = typeNames.Contains(category);
        _uiDrawn = false;
    }

    public override bool _ParseProperty(GodotObject @object, Variant.Type type,
        string name, PropertyHint hintType, string hintString,
        PropertyUsageFlags usageFlags, bool wide)
    {
        if (_inTargetCategory)
        {
            if (!_uiDrawn && name == NodeComponents.LAST_PLACEHOLDER_NAME)
            {
                _uiDrawn = true;
                _DrawNodeComponentsUI();
            }
        }
        return false;
    }

    private void _OnAddComponentPressed()
    {
        GD.Print("Add Component Pressed");
    }

    private static Texture2D _GetEditorIcon(StringName iconName)
    {
        var gui = EditorInterface.Singleton.GetBaseControl();
        return gui.GetThemeIcon(iconName, "EditorIcons");
    }
}
#endif
