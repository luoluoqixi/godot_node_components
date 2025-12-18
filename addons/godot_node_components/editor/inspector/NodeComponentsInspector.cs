#if TOOLS
using Godot;

namespace GodotNodeComponents.Editor;

public partial class NodeComponentsInspector : EditorInspectorPlugin
{
    private GodotObject currentObject;

    public override bool _CanHandle(GodotObject @object)
    {
        return typeof(IComponents).IsAssignableFrom(@object.GetType());
    }

    private void _DrawComponent(VBoxContainer section, ComponentsController componentsController, int index)
    {
        var component = componentsController.GetComponentIndex(index);
        var componentType = component?.GetType();
        var typeFullName = componentType == null ? "" : componentType.FullName;
        var typeName = componentType == null ? "Null" : componentType.Name;

        var path = componentsController.Owner.GetPath();
        var saveKey = $"{path}/node_components/component_{index}_{typeFullName}";
        // var componentsCountLabel = new Label
        // {
        //     Text = "Component: " + index,
        //     HorizontalAlignment = HorizontalAlignment.Left,
        // };

        var popup = new PopupMenu();
        popup.AddItem("Reset", 0);
        popup.AddSeparator();
        popup.AddItem("Remove Component", 1);
        popup.AddItem("Move Up", 2);
        popup.AddItem("Move Down", 3);

        popup.IdPressed += _ContextMeenu_IdPressed;

        var group = EditorGUIUtility.DrawCollapsibleGroup(typeName, 1, popup, saveKey);
        section.AddChild(group);
    }

    public override void _ParseBegin(GodotObject @object)
    {
        base._ParseBegin(@object);

        if (@object is not IComponents) return;

        currentObject = @object;

        var componentsInterface = @object as IComponents;
        var componentsController = componentsInterface.Components;

        if (componentsController == null) return;

        var section = new VBoxContainer();
        {
            var header = EditorGUIUtility.DrawHeaderContainer("Node Components");
            section.AddChild(header);
        }

        {
            // Components
            var componentCount = componentsController.Count;

            var componentsCountLabel = new Label
            {
                Text = "Count: " + componentCount,
                HorizontalAlignment = HorizontalAlignment.Left,
            };
            section.AddChild(componentsCountLabel);

            for (int i = 0; i < componentCount; i++)
            {
                _DrawComponent(section, componentsController, i);
            }
        }

        {
            var addIcon = EditorThemeUtility.GetEditorIcon("Add");
            var addComponent = EditorGUIUtility.DrawButton("Add Component", addIcon, _OnAddComponentPressed);
            section.AddChild(addComponent);
        }

        AddCustomControl(section);
    }

    private void _ContextMeenu_IdPressed(long id)
    {
        GD.Print("Context menu item pressed: " + id);
    }

    private void _OnAddComponentPressed()
    {
        if (currentObject == null) return;
        if (currentObject is not IComponents) return;

        var componentsController = (currentObject as IComponents).Components;
        if (componentsController == null) return;

        var node = componentsController.Owner;
        if (node == null) return;
        if (!IsInstanceValid(node)) return;

        var undoRedo = EditorGUIUtility.GetUndoRedo();
        if (undoRedo == null) return;

        var componentType = typeof(SimpleComponent);
        if (componentType == null) return;

        string nodeName = node.Name;
        string componentTypeName = componentType.Name;

        var oldData = componentsController.GetComponentsToData();
        undoRedo.CreateAction("Add Component " + nodeName + ": " + componentTypeName);

        // https://github.com/godotengine/godot/issues/90430
        var command = new UndoRedoCommand();
        command.AddDoMethod(() =>
        {
            // GD.PrintErr("Add Component");
            if (componentsController == null) return;
            if (node == null) return;
            if (!IsInstanceValid(node)) return;

            componentsController.AddComponent(componentType);
            componentsController.ApplyComponents();
            node.NotifyPropertyListChanged();
        });
        command.AddUndoMethod(() =>
        {
            // GD.PrintErr("Revert Component");
            if (componentsController == null) return;
            if (node == null) return;
            if (!IsInstanceValid(node)) return;

            componentsController.RevertComponentsFromData(oldData);
            componentsController.ApplyComponents();
            node.NotifyPropertyListChanged();
        });
        command.AddToUndoRedo(undoRedo);
        undoRedo.CommitAction();
    }
}
#endif
