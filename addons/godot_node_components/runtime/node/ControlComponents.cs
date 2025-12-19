using Godot;

namespace GodotNodeComponents;

[Tool]
public partial class ControlComponents : Control, IComponents
{
    public ComponentsController Components { get; set; }

    public ControlComponents()
    {
        if (Components == null)
            Components = new ComponentsController();
        Components.Initialize(this);
    }

    public override void _EnterTree()
    {
        Components.EnterTree();
    }

    public override void _ExitTree()
    {
        Components.ExitTree();
    }

    public override void _Ready()
    {
        Components.Ready();
    }

    public override void _Process(double delta)
    {
        Components.Process(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        Components.PhysicsProcess(delta);
    }

    protected override void Dispose(bool disposing)
    {
        Components.Dispose();
        Components = null;
    }

    public void SaveComponents(string[] data)
    {
        this.SaveComponentsToMeta(data);
    }

    public string[] LoadComponents()
    {
        return this.LoadComponentsFromMeta();
    }
}
