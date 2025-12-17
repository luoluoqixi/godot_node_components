using Godot;

namespace GodotNodeComponents.Examples;

public class ExampleComponent : BaseComponent
{
    public override void Start()
    {
        GD.Print("ExampleComponent Start: Hello " + Owner.Name);
    }
}
