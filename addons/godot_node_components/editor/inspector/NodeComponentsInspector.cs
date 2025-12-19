#if TOOLS
using System;
using System.Linq;
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
        popup.AddItem("Reset ", 0);
        popup.AddSeparator();
        popup.AddItem("Remove Component ", 1);
        popup.AddItem("Move Up ", 2);
        popup.AddItem("Move Down ", 3);

        popup.IdPressed += (id) => _ContextMeenu_IdPressed(id, index);

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

    private void _ResetComponent(ComponentsController cc, Node node, int index, UndoRedo undoRedo)
    {
        string nodeName = node.Name;

        var oldData = cc.GetComponentsToData();
        undoRedo.CreateAction("Reset Component " + nodeName + ": " + index);

        // https://github.com/godotengine/godot/issues/90430
        var command = new UndoRedoCommand();
        command.AddDoMethod(() =>
        {
            if (cc == null) return;
            if (node == null) return;
            if (!IsInstanceValid(node)) return;

            cc.ResetComponent(index);
            cc.ApplyComponents();
            node.NotifyPropertyListChanged();

            EditorGUIUtility.MarkSceneAsUnsaved();
        });
        command.AddUndoMethod(() =>
        {
            if (cc == null) return;
            if (node == null) return;
            if (!IsInstanceValid(node)) return;

            cc.RevertComponentsFromData(oldData);
            cc.ApplyComponents();
            node.NotifyPropertyListChanged();

            EditorGUIUtility.MarkSceneAsUnsaved();
        });
        command.AddToUndoRedo(undoRedo);
        undoRedo.CommitAction();
    }

    private void _MoveUpComponent(ComponentsController cc, Node node, int index, UndoRedo undoRedo)
    {
        string nodeName = node.Name;

        var oldData = cc.GetComponentsToData();
        undoRedo.CreateAction("MoveUp Component " + nodeName + ": " + index);

        // https://github.com/godotengine/godot/issues/90430
        var command = new UndoRedoCommand();
        command.AddDoMethod(() =>
        {
            if (cc == null) return;
            if (node == null) return;
            if (!IsInstanceValid(node)) return;

            cc.MoveUpComponent(index);
            cc.ApplyComponents();
            node.NotifyPropertyListChanged();

            EditorGUIUtility.MarkSceneAsUnsaved();
        });
        command.AddUndoMethod(() =>
        {
            if (cc == null) return;
            if (node == null) return;
            if (!IsInstanceValid(node)) return;

            cc.RevertComponentsFromData(oldData);
            cc.ApplyComponents();
            node.NotifyPropertyListChanged();

            EditorGUIUtility.MarkSceneAsUnsaved();
        });
        command.AddToUndoRedo(undoRedo);
        undoRedo.CommitAction();
    }

    private void _MoveDownComponent(ComponentsController cc, Node node, int index, UndoRedo undoRedo)
    {
        string nodeName = node.Name;

        var oldData = cc.GetComponentsToData();
        undoRedo.CreateAction("MoveDown Component " + nodeName + ": " + index);

        // https://github.com/godotengine/godot/issues/90430
        var command = new UndoRedoCommand();
        command.AddDoMethod(() =>
        {
            if (cc == null) return;
            if (node == null) return;
            if (!IsInstanceValid(node)) return;

            cc.MoveDownComponent(index);
            cc.ApplyComponents();
            node.NotifyPropertyListChanged();

            EditorGUIUtility.MarkSceneAsUnsaved();
        });
        command.AddUndoMethod(() =>
        {
            if (cc == null) return;
            if (node == null) return;
            if (!IsInstanceValid(node)) return;

            cc.RevertComponentsFromData(oldData);
            cc.ApplyComponents();
            node.NotifyPropertyListChanged();

            EditorGUIUtility.MarkSceneAsUnsaved();
        });
        command.AddToUndoRedo(undoRedo);
        undoRedo.CommitAction();
    }

    private void _RemoveComponent(ComponentsController cc, Node node, int index, UndoRedo undoRedo)
    {
        string nodeName = node.Name;

        var oldData = cc.GetComponentsToData();
        undoRedo.CreateAction("Remove Component " + nodeName + ": " + index);

        // https://github.com/godotengine/godot/issues/90430
        var command = new UndoRedoCommand();
        command.AddDoMethod(() =>
        {
            if (cc == null) return;
            if (node == null) return;
            if (!IsInstanceValid(node)) return;

            cc.RemoveComponentAt(index);
            cc.ApplyComponents();
            node.NotifyPropertyListChanged();

            EditorGUIUtility.MarkSceneAsUnsaved();
        });
        command.AddUndoMethod(() =>
        {
            if (cc == null) return;
            if (node == null) return;
            if (!IsInstanceValid(node)) return;

            cc.RevertComponentsFromData(oldData);
            cc.ApplyComponents();
            node.NotifyPropertyListChanged();

            EditorGUIUtility.MarkSceneAsUnsaved();
        });
        command.AddToUndoRedo(undoRedo);
        undoRedo.CommitAction();
    }

    private void _ContextMeenu_IdPressed(long id, int index)
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

        if (id == 0)
        {
            // Reset
            _ResetComponent(componentsController, node, index, undoRedo);
        }
        else if (id == 1)
        {
            // Remove Component
            _RemoveComponent(componentsController, node, index, undoRedo);
        }
        else if (id == 2)
        {
            // Move Up
            _MoveUpComponent(componentsController, node, index, undoRedo);
        }
        else if (id == 3)
        {
            // Move Down
            _MoveDownComponent(componentsController, node, index, undoRedo);
        }
    }

    private void _DoAddComponent(string typeName)
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

        var componentType = Type.GetType(typeName);
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

            EditorGUIUtility.MarkSceneAsUnsaved();
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

            EditorGUIUtility.MarkSceneAsUnsaved();
        });
        command.AddToUndoRedo(undoRedo);
        undoRedo.CommitAction();
    }

    private void _OnAddComponentPressed()
    {
        var typeNames = CommonAttributeUtility.GetAllTypeNames<BaseComponent>();
        Window window = null;
        window = EditorGUIUtility.OpenPopupWindow("Add Component", new Vector2I(800, 600), (vbox) =>
        {
            {
                var input = new LineEdit();
                input.PlaceholderText = "Search...";
                input.ClearButtonEnabled = true;
                input.TreeEntered += () =>
                {
                    if (input.IsInsideTree())
                        input.GrabFocus();
                };
                vbox.AddChild(input);
            }

            int count = typeNames.Length;
            var tree = new Tree();
            tree.HideRoot = true;
            var root = tree.CreateItem();

            tree.SetAnchorsPreset(Control.LayoutPreset.FullRect);
            tree.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            tree.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
            tree.SelectMode = Tree.SelectModeEnum.Single;
            tree.AddThemeFontOverride("font", EditorThemeUtility.GetFont("bold", "EditorFonts"));
            tree.AddThemeFontSizeOverride("font_size", EditorThemeUtility.GetFontSize("bold_size", "EditorFonts"));
            tree.AddThemeColorOverride("font_color", EditorThemeUtility.GetColor("font_color", "Tree"));
            for (var i = 0; i < count; i++)
            {
                var item = tree.CreateItem(root);
                var fullName = typeNames[i];
                item.SetText(0, "\n" + fullName + "\n\n");
                item.SetMeta("value", fullName);
            }
            vbox.AddChild(tree);
            if (count > 0)
            {
                root.GetChild(0).Select(0);
            }

            {
                var addComponent = EditorGUIUtility.DrawButton("Ok", null, () =>
                {
                    if (tree == null) return;
                    if (!tree.IsInsideTree()) return;
                    var select = tree.GetSelected();
                    var value = select.GetMeta("value");
                    _DoAddComponent(value.AsString());
                    if (window != null && window.IsInsideTree())
                    {
                        window.QueueFree();
                    }
                }, true);
                vbox.AddChild(addComponent);
            }
        });
    }
}
#endif
