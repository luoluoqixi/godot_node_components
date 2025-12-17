#if TOOLS
using Godot;

namespace GodotNodeComponents.Editor;

public partial class NodeComponentsInspector : EditorInspectorPlugin
{
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
        var group = EditorGUIUtility.DrawCollapsibleGroup(typeName, 1, saveKey);

        section.AddChild(group);
    }

    private void _DrawNodeComponentsUI(GodotObject obj)
    {
        if (obj is not IComponents) return;

        var componentsInterface = obj as IComponents;
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

            // var componentsCountLabel = new Label
            // {
            //     Text = "Count: " + componentCount,
            //     HorizontalAlignment = HorizontalAlignment.Left,
            // };
            // section.AddChild(componentsCountLabel);

            for (int i = 0; i < componentCount; i++)
            {
                _DrawComponent(section, componentsController, i);
            }
        }

        {
            var addIcon = EditorThemeUtility.GetEditorIcon("Add");
            var addComponent = EditorGUIUtility.DrawButton("Add Component", addIcon, () => _OnAddComponentPressed(componentsController));
            section.AddChild(addComponent);
        }

        AddCustomControl(section);
    }

    public override void _ParseBegin(GodotObject @object)
    {
        base._ParseBegin(@object);
        _DrawNodeComponentsUI(@object);
    }

    private void _OnAddComponentPressed(ComponentsController componentsController)
    {
        componentsController.AddComponent<SimpleComponent>();
        componentsController.ApplyComponents();
        if (componentsController.Owner is Node node)
        {
            node.NotifyPropertyListChanged();
        }
    }
}
#endif
