using System;
using System.Collections.Generic;
using System.Text.Json;
using Godot;

namespace GodotNodeComponents;

public class ComponentsController : IDisposable
{
    private bool _initialized = false;
    private List<BaseComponent> _components;

    private IComponents _componentsInterface;
    private Node _owner;
    public Node Owner => _owner;

    public int Count => _components?.Count ?? 0;


    public void Initialize<T>(T owner)
        where T : Node, IComponents
    {
        _owner = owner;
        _componentsInterface = owner;
    }

    public void EnterTree()
    {
        _initialized = true;
        InitComponents();
        if (Engine.IsEditorHint())
        {
            _EditorEnterTree();
            return;
        }
        _EnterTree();
    }

    public void ExitTree()
    {
        if (Engine.IsEditorHint())
        {
            _EditorExitTree();
            return;
        }
        _ExitTree();
    }

    public void Ready()
    {
        GD.Print("Ready");
        if (Engine.IsEditorHint())
        {
            _EditorReady();
            return;
        }
        _Ready();
    }

    public void Process(double delta)
    {
        if (Engine.IsEditorHint())
        {
            if (!_initialized)
            {
                _initialized = true;
                InitComponents();
            }
            _EditorProcess(delta);
            return;
        }
        _Process(delta);
    }

    public virtual void _EnterTree()
    {

    }

    public virtual void _EditorEnterTree()
    {

    }

    public virtual void _ExitTree()
    {

    }

    public virtual void _EditorExitTree()
    {

    }

    public virtual void _Ready()
    {

    }

    public virtual void _EditorReady()
    {

    }

    public virtual void _Process(double delta)
    {

    }

    public virtual void _EditorProcess(double delta)
    {

    }

    public void Dispose()
    {

    }

    public void AddComponent<T>()
        where T : BaseComponent, new()
    {
        if (_components == null)
            _components = new List<BaseComponent>();
        var component = new T();
        _components.Add(component);
    }

    public void InitComponents()
    {
        LoadComponents();
    }

    public void ApplyComponents()
    {
        var data = _SaveComponentsToData();
        _componentsInterface.SaveComponents(data);
    }

    public void LoadComponents()
    {
        var data = _componentsInterface.LoadComponents();
        _LoadComponentsFromData(data);
    }

    private string[] _SaveComponentsToData()
    {
        if (_components == null)
            return null;
        int count = _components.Count;
        if (count == 0)
            return null;
        var data = new string[count];
        for (int i = 0; i < count; i++)
        {
            var component = _components[i];
            try
            {
                data[i] = SerializeComponent(component, i);
            }
            catch (Exception e)
            {
                GD.PrintErr($"Node {_owner.Name}, failed to serialize component at index {i}: {e}");
                data[i] = "";
            }
        }
        return data;
    }

    private void _LoadComponentsFromData(string[] data)
    {
        if (data == null)
        {
            _components = null;
            return;
        }
        _components = new List<BaseComponent>();
        int count = data.Length;
        for (int i = 0; i < count; i++)
        {
            var componentData = data[i];
            try
            {
                var component = ParseComponent(componentData, i);
                _components.Add(component);
            }
            catch (Exception e)
            {
                GD.PrintErr($"Node {_owner.Name}, failed to parse component at index {i}: {e}");
            }
        }
    }

    protected virtual BaseComponent ParseComponent(string data, int index)
    {
        if (string.IsNullOrEmpty(data))
        {
            GD.PrintErr($"Node {_owner.Name}, component data: {index} is null or empty.");
            return null;
        }
        int i = data.IndexOf('|');
        if (i < 0)
        {
            GD.PrintErr($"Node {_owner.Name}, component data: {index} is invalid.");
            return null;
        }
        var typeName = data[..i];
        var componentData = data[(i + 1)..];
        var type = Type.GetType(typeName);
        if (type == null)
        {
            GD.PrintErr($"Node {_owner.Name}, component data: {index} type {typeName} not found.");
            return null;
        }
        var component = JsonSerializer.Deserialize(componentData, type);
        NodeComponentsUtility.ClearJsonCache();
        if (component == null)
        {
            GD.PrintErr($"Node {_owner.Name}, component data: {index} deserialized to null.");
            return null;
        }
        var result = component as BaseComponent;
        if (result == null)
        {
            GD.PrintErr($"Node {_owner.Name}, component data: {index} is not a BaseComponent.");
            return null;
        }
        return result;
    }

    protected virtual string SerializeComponent(BaseComponent component, int index)
    {
        if (component == null)
            return "";
        JsonSerializerOptions options = new() { };
        string json = JsonSerializer.Serialize(component, options);
        NodeComponentsUtility.ClearJsonCache();
        var type = component.GetType();
        return type.FullName + "|" + json;
    }
}
