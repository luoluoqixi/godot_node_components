#if TOOLS
using Godot;

namespace GodotNodeComponents.Editor;

[Tool]
public partial class NodeComponentsPlugin : EditorPlugin
{
    private CommonAttributeInspector _commonAttributeInspector;
    private NodeComponentsInspector _nodeComponentsInspector;

    public override void _EnterTree()
    {
        _commonAttributeInspector = new CommonAttributeInspector();
        _nodeComponentsInspector = new NodeComponentsInspector();
        AddInspectorPlugin(_commonAttributeInspector);
        AddInspectorPlugin(_nodeComponentsInspector);
    }

    public override void _ExitTree()
    {
        RemoveInspectorPlugin(_commonAttributeInspector);
        RemoveInspectorPlugin(_nodeComponentsInspector);
    }
}
#endif
