using System.Collections.Generic;
using Godot;

namespace GodotNodeComponents;

[Tool]
public partial class NodeComponents : Node
{
    internal static readonly string LAST_PLACEHOLDER_NAME = "__innerNodeComponentsLastPlaceholder";

    private SerializedType _serializationType = SerializedType.Json;

    [Export]
    public SerializedType SerializationType
    {
        get => _serializationType;
        set => _SetSerializedType(value);
    }

    private List<NodeComponent> _components = [];

    [Export][HideInInspector] private string __innerNodeComponentsJsonData;
    [Export][HideInInspector] private byte[] __innerNodeComponentsBinaryData;

    // 占位符, 用来标记当前组件渲染区域的最后一个属性, 便于在编辑器中添加自定义UI
    [Export][HideInInspector] private byte __innerNodeComponentsLastPlaceholder;

    public override void _EnterTree()
    {
        // GD.Print("_EnterTree called");
        base._EnterTree();
    }

    private void _SetSerializedType(SerializedType newType)
    {
        var oldType = _serializationType;
        _serializationType = newType;
        if (oldType != newType)
        {
            GD.Print("SerializationType changed to:", _serializationType);
        }
    }
}
