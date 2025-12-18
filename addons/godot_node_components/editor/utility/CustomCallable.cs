#if TOOLS
using System;
using Godot;

namespace GodotNodeComponents.Editor;

public partial class CustomCallable : RefCounted
{
    private Action _action;

    public static CustomCallable From(Action action)
    {
        var customCallable = new CustomCallable();
        customCallable._action = action;
        return customCallable;
    }

    public void AddAction(Action action)
    {
        _action += action;
    }

    public void RemoveAction(Action action)
    {
        _action -= action;
    }

    public void Call()
    {
        _action?.Invoke();
    }

    public Callable GetCallable()
    {
        return new Callable(this, nameof(Call));
    }
}
#endif
