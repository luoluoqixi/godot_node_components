## Godot Node Components

A Godot C# plugin that enables a Node to attach multiple Component scripts.

[中文](./README_zh.md)


### Godot Version

- Godot_v4.6-beta1_mono


### Quick Start

Attach the `NodeComponents` component to a Node, click "Add Component", select the component to attach, and click OK.

<p>
    <img src="./docs/1.png" width="48%" />
    <img src="./docs/2.png" width="48%" />
</p>

Example implementation of the `ExampleComponent`:

```csharp
public class ExampleComponent : BaseComponent
{
    public override void Awake()
    {
        GD.Print("ExampleComponent Awake: " + Owner.Name);
    }

    public override void Start()
    {
        GD.Print("ExampleComponent Start: " + Owner.Name);
    }

    public override void Update(double delta)
    {

    }
}
```

When using other node types, such as `Node3D`, you should use the corresponding `Node3DComponents`. Currently only `NodeComponents`, `Node2DComponents`, `Node3DComponents` and `ControlComponents` are implemented.

If you need more types, you can implement them yourself by inheriting the corresponding class (Godot uses the base class to attach to the Node and cannot access the original Node's type. [See details](https://github.com/godotengine/godot/issues/11980)). For example, for `Sprite2D`：

```csharp
[Tool]
public partial class Sprite2DComponents : Sprite2D, IComponents
{
    public ComponentsController Components { get; set; }

    public Sprite2DComponents()
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
```

### How is it implemented?

The Components data is serialized into Json, stored in a `string[]`, and then saved in the node's Meta data, where it is restored at runtime.

```csharp
public static void SaveComponentsToMeta(this Node node, string[] data)
{
    node.SetMeta(ComponentsConfig.ComponentsMetaKey, data);
}
```

### Not Yet Implemented

- Component fields cannot yet be edited in the Inspector.
- After recompilation, Godot's plugin reports some strange errors due to some anonymous callbacks, which do not affect runtime for now.
