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
        var componentsCountLabel = new Label
        {
            Text = "Component: " + index,
            HorizontalAlignment = HorizontalAlignment.Left,
        };
        section.AddChild(componentsCountLabel);
    }

    private void _DrawNodeComponentsUI(GodotObject obj)
    {
        if (obj is not IComponents) return;

        var componentsInterface = obj as IComponents;
        var componentsController = componentsInterface.Components;

        if (componentsController == null) return;

        var section = new VBoxContainer();
        {
            var titleContainer = new PanelContainer();
            titleContainer.AddThemeStyleboxOverride("panel", EditorThemeUtility.GetStylebox("bg", "EditorInspectorCategory"));
            {
                var titleHbox = new HBoxContainer();
                titleHbox.Alignment = BoxContainer.AlignmentMode.Center;
                {
                    var titleIcon = new TextureRect();
                    titleIcon.Texture = EditorThemeUtility.GetEditorIcon("Node");
                    titleIcon.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
                    titleHbox.AddChild(titleIcon);
                }
                {
                    var titleLabel = new Label
                    {
                        Text = "Node Components",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        LabelSettings = new LabelSettings
                        {
                            Font = EditorThemeUtility.GetFont("bold", "EditorFonts"),
                            FontSize = EditorThemeUtility.GetFontSize("bold_size", "EditorFonts"),
                            FontColor = EditorThemeUtility.GetColor("font_color", "Tree"),
                        }
                    };
                    titleHbox.AddChild(titleLabel);
                }
                titleContainer.AddChild(titleHbox);
            }

            section.AddChild(titleContainer);
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
            var addComponent = new Button();
            addComponent.SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter;
            addComponent.Text = "Add Component";
            addComponent.Icon = EditorThemeUtility.GetEditorIcon("Add");
            addComponent.Pressed += () => _OnAddComponentPressed(componentsController);
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
