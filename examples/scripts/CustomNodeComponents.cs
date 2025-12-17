using Godot;

namespace GodotNodeComponents.Examples;

[Tool]
public partial class CustomNodeComponents : NodeComponents
{
    [Export] private string _customProperty = "Hello, Node Components!";

    public override void _Ready()
    {
        base._Ready();

        var components = Components.GetComponents<BaseComponent>();
        foreach (var component in components)
        {
            GD.Print($"Component {component.Name}: {component.GetType().Name}");
        }
    }
}
