using System;
using System.Collections.Generic;
using System.Text.Json;
using Godot;

namespace GodotNodeComponents;

public partial class ComponentsController : IDisposable
{
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
        _InitComponents();
    }

    public void EnterTree()
    {
        _InitComponents();
        if (Engine.IsEditorHint())
        {
            _EditorEnterTree();
            return;
        }
        _EnterTree();
    }

    public void ExitTree()
    {
        _DestroyComponents();
        if (Engine.IsEditorHint())
        {
            _EditorExitTree();
            return;
        }
        _ExitTree();
    }

    public void Ready()
    {
        if (Engine.IsEditorHint())
        {
            _EditorReady();
            return;
        }
        _StartComponents();
        _Ready();
    }

    public void Process(double delta)
    {
        // GD.Print("ComponentsController Process ", _components?.Count);
        if (Engine.IsEditorHint())
        {
            _EditorProcess(delta);
            return;
        }
        _UpdateComponents(delta);
        _Process(delta);
    }

    public void PhysicsProcess(double delta)
    {
        if (Engine.IsEditorHint())
        {
            _EditorPhysicsProcess(delta);
            return;
        }
        _FixedUpdateComponents(delta);
        _PhysicsProcess(delta);
    }

    public void Dispose()
    {
        _components = null;
        _componentsInterface = null;
        _owner = null;
    }

    public virtual void _EnterTree() { }

    public virtual void _EditorEnterTree() { }

    public virtual void _ExitTree() { }

    public virtual void _EditorExitTree() { }

    public virtual void _Ready() { }

    public virtual void _EditorReady() { }

    public virtual void _Process(double delta) { }

    public virtual void _EditorProcess(double delta) { }

    public virtual void _PhysicsProcess(double delta) { }

    public virtual void _EditorPhysicsProcess(double delta) { }

    public T AddComponent<T>()
        where T : BaseComponent, new()
    {
        if (_components == null)
            _components = new List<BaseComponent>();
        var component = new T();
        _components.Add(component);
        _OnComponentCreate(component);
        return component;
    }

    public BaseComponent AddComponent(Type type)
    {
        if (_components == null)
            _components = new List<BaseComponent>();
        var component = (BaseComponent)Activator.CreateInstance(type);
        if (component == null)
        {
            throw new Exception($"Failed to create component of type {type.FullName}");
        }
        _components.Add(component);
        _OnComponentCreate(component);
        return component;
    }

    public T GetComponent<T>()
        where T : BaseComponent
    {
        if (_components == null)
            return null;
        foreach (var component in _components)
        {
            if (component is T tComponent)
            {
                return tComponent;
            }
        }
        return null;
    }

    public BaseComponent GetComponent(Type type)
    {
        if (_components == null)
            return null;
        foreach (var component in _components)
        {
            var componentType = component.GetType();
            if (componentType == type || componentType.IsSubclassOf(type))
            {
                return component;
            }
        }
        return null;
    }

    public T[] GetComponents<T>()
        where T : BaseComponent
    {
        if (_components == null)
            return [];
        var result = new List<T>();
        foreach (var component in _components)
        {
            if (component is T tComponent)
            {
                result.Add(tComponent);
            }
        }
        return result.ToArray();
    }

    public BaseComponent[] GetComponents(Type type)
    {
        if (_components == null)
            return [];
        var result = new List<BaseComponent>();
        foreach (var component in _components)
        {
            var componentType = component.GetType();
            if (componentType == type || componentType.IsSubclassOf(type))
            {
                result.Add(component);
            }
        }
        return result.ToArray();
    }

    public BaseComponent GetComponentIndex(int index)
    {
        if (_components == null)
            return null;
        if (index < 0 || index >= _components.Count)
            return null;
        return _components[index];
    }

    private void _InitComponents()
    {
        var data = _componentsInterface.LoadComponents();
        _LoadComponentsFromData(data);
    }

    private void _DestroyComponents()
    {
        if (_components == null)
            return;
        foreach (var component in _components)
        {
            if (component == null) continue;
            try
            {
                if (component.Enabled)
                {
                    component.Enabled = false;
                }
                component.OnDestroy();
            }
            catch (Exception e)
            {
                GD.PrintErr($"Node {_owner.Name}, failed to destroy component {component.GetType().FullName}: {e}");
            }
        }
        _components.Clear();
        _components = null;
    }

    private void _StartComponents()
    {
        if (_components == null)
            return;
        foreach (var component in _components)
        {
            if (component == null) continue;
            try
            {
                component.Start();
            }
            catch (Exception e)
            {
                GD.PrintErr($"Node {_owner.Name}, failed to start component {component.GetType().FullName}: {e}");
            }
        }
    }

    private void _UpdateComponents(double delta)
    {
        if (_components == null)
            return;
        foreach (var component in _components)
        {
            if (component == null) continue;
            try
            {
                component.Update(delta);
            }
            catch (Exception e)
            {
                GD.PrintErr($"Node {_owner.Name}, failed to update component {component.GetType().FullName}: {e}");
            }
        }
    }

    private void _FixedUpdateComponents(double delta)
    {
        if (_components == null)
            return;
        foreach (var component in _components)
        {
            if (component == null) continue;
            try
            {
                component.FixedUpdate(delta);
            }
            catch (Exception e)
            {
                GD.PrintErr($"Node {_owner.Name}, failed to fixed update component {component.GetType().FullName}: {e}");
            }
        }
    }

    public void ApplyComponents()
    {
        var data = _SaveComponentsToData();
        _componentsInterface.SaveComponents(data);
    }

    internal string[] GetComponentsToData()
    {
        var data = _SaveComponentsToData();
        return data;
    }

    internal void RevertComponentsFromData(string[] data)
    {
        _LoadComponentsFromData(data, false);
    }

    protected void _OnComponentCreate(BaseComponent component)
    {
        component._SetOwner(_owner);
        try
        {
            component.Awake();
            if (component.Enabled)
            {
                component.OnEnable();
            }
        }
        catch (Exception e)
        {
            GD.PrintErr($"Node {_owner.Name}, failed to initialize component {component.GetType().FullName}: {e}");
        }
    }

    protected virtual string[] _SaveComponentsToData()
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

    protected virtual void _LoadComponentsFromData(string[] data, bool invokeCreateEvent = true)
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
                if (component != null)
                {
                    if (invokeCreateEvent)
                    {
                        _OnComponentCreate(component);
                    }
                }
            }
            catch (Exception e)
            {
                GD.PrintErr($"Node {_owner.Name}, failed to parse component at index {i}: {e}");
            }
        }
    }

    protected virtual JsonSerializerOptions GetJsonSerializerOptions()
    {
        return new JsonSerializerOptions
        {
            // 默认包含公共字段, 这和 Unity 的默认序列化行为一致
            IncludeFields = true,
            // 即使序列化为更漂亮的格式, 存储到 .tscn 文件中也会以字符串形式存储, 对可读性没有帮助
            WriteIndented = false,
            // 忽略只读字段和属性
            IgnoreReadOnlyFields = true,
            IgnoreReadOnlyProperties = true,
        };
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
        var options = GetJsonSerializerOptions();
        var component = JsonSerializer.Deserialize(componentData, type, options);
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
        var options = GetJsonSerializerOptions();
        string json = JsonSerializer.Serialize(component, component.GetType(), options);
        NodeComponentsUtility.ClearJsonCache();
        var type = component.GetType();
        return type.FullName + "|" + json;
    }
}
