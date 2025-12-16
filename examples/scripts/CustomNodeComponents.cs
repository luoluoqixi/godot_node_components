using Godot;

namespace GodotNodeComponents.Examples;

[Tool]
public partial class CustomNodeComponents : NodeComponents
{
    [Export] private string _customProperty = "Hello, Node Components!";

    public override void _Ready()
    {
        base._Ready();
        GD.Print("NodeTest is ready.");
    }
}
