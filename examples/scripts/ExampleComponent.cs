using Godot;

namespace GodotNodeComponents.Examples;

public class ExampleComponent : BaseComponent
{
    public float speed = 1.0f;
    private Node3D _node3D;

    public override void Awake()
    {
        base.Awake();
        GD.Print("ExampleComponent Awake: " + Owner.Name);
    }
    public override void Start()
    {
        GD.Print("ExampleComponent Start: " + Owner.Name);
        _node3D = Owner as Node3D;
    }

    public override void Update(double delta)
    {
        base.Update(delta);
        if (_node3D != null)
        {
            _node3D.RotateY((float)(delta * 0.5 * speed));
        }
    }
}
