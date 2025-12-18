#if TOOLS
using Godot;

namespace GodotNodeComponents.Editor;

[Tool]
public partial class NodeComponentsPlugin : EditorPlugin
{
    private CommonAttributeInspector _commonAttributeInspector;
    private NodeComponentsInspector _nodeComponentsInspector;

    private static UndoRedo _undoRedo = new UndoRedo();
    public static UndoRedo UndoRedo => _undoRedo;

    public override void _EnterTree()
    {
        _commonAttributeInspector = new CommonAttributeInspector();
        _nodeComponentsInspector = new NodeComponentsInspector();
        AddInspectorPlugin(_commonAttributeInspector);
        AddInspectorPlugin(_nodeComponentsInspector);

        SetProcessUnhandledKeyInput(true);
    }

    public override void _ExitTree()
    {
        RemoveInspectorPlugin(_commonAttributeInspector);
        RemoveInspectorPlugin(_nodeComponentsInspector);
        _undoRedo = null;
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && !keyEvent.Pressed)
        {
            if (keyEvent.Keycode == Key.Z && (keyEvent.CtrlPressed || keyEvent.MetaPressed))
            {
                if (_undoRedo.HasUndo())
                {
                    _undoRedo.Undo();
                }
            }
            else if (keyEvent.Keycode == Key.Y && (keyEvent.CtrlPressed || keyEvent.MetaPressed))
            {
                if (_undoRedo.HasRedo())
                {
                    _undoRedo.Redo();
                }
            }
        }
    }
}
#endif
