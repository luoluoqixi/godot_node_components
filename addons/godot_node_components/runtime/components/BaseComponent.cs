using System.Text.Json.Serialization;
using Godot;

namespace GodotNodeComponents;

public abstract class BaseComponent
{
    [JsonIgnore]
    public string Name => _owner.Name;
    [JsonIgnore]
    private Node _owner;
    [JsonIgnore]
    public Node Owner => _owner;
    [JsonIgnore]
    private bool _enabled = true;
    [JsonIgnore]
    public bool Enabled
    {
        get => _enabled;
        set => _SetEnabled(value);
    }

    internal void _SetOwner(Node owner)
    {
        _owner = owner;
    }

    private void _SetEnabled(bool enabled)
    {
        if (_enabled == enabled)
            return;

        _enabled = enabled;
        if (_enabled)
            OnEnable();
        else
            OnDisable();
    }

    public virtual void Awake() { }

    public virtual void OnEnable() { }

    public virtual void Start() { }

    public virtual void Update(double delta) { }

    public virtual void FixedUpdate(double delta) { }

    public virtual void OnDisable() { }

    public virtual void OnDestroy() { }
}
