#if TOOLS
using System;
using Godot;

namespace GodotNodeComponents.Editor;

public partial class UndoRedoCommand : RefCounted
{
    public Action DoAction { get; private set; }
    public Action UndoAction { get; private set; }

    public void AddDoMethod(Action doAction)
    {
        DoAction += doAction;
    }

    public void AddUndoMethod(Action undoAction)
    {
        UndoAction += undoAction;
    }

    public void CallDo()
    {
        DoAction?.Invoke();
    }

    public void CallUndo()
    {
        UndoAction?.Invoke();
    }

    public Callable GetDoCallable()
    {
        return new Callable(this, nameof(CallDo));
    }

    public Callable GetUndoCallable()
    {
        return new Callable(this, nameof(CallUndo));
    }

    public void AddToUndoRedo(UndoRedo undoRedo)
    {
        undoRedo.AddDoMethod(GetDoCallable());
        undoRedo.AddUndoMethod(GetUndoCallable());
    }
}
#endif
