#if TOOLS
using System;
using Godot;

namespace GodotNodeComponents.Editor;

internal static class EditorGUIUtility
{
    public static VBoxContainer DrawCollapsibleGroup(string title, int level = 1, string savePath = null)
    {
        var group = new VBoxContainer();

        var expanded = false;

        var headerButton = new Button();
        headerButton.Flat = true;
        headerButton.ToggleMode = true;
        headerButton.ButtonPressed = expanded;
        headerButton.Alignment = HorizontalAlignment.Left;
        headerButton.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        headerButton.AddThemeFontOverride("font", EditorThemeUtility.GetFont("bold", "EditorFonts"));
        // GD.Print(EditorThemeUtility.GetFontSize("bold_size", "EditorFonts"));
        headerButton.AddThemeFontSizeOverride("font_size", 28);

        var updateHeader = (string title, int level) =>
        {
            headerButton.Icon = expanded
                ? EditorThemeUtility.GetIcon("arrow", "Tree")
                : EditorThemeUtility.GetIcon("arrow_collapsed", "Tree");

            string indent = new string(' ', (level - 1) * 2);
            headerButton.Text = $"{indent}{title}";
        };

        updateHeader(title, level);

        group.AddChild(headerButton);

        var content = new VBoxContainer();
        content.Visible = expanded;
        group.AddChild(content);

        var styleBox = new StyleBoxFlat();
        headerButton.AddThemeStyleboxOverride("normal", styleBox);

        headerButton.Toggled += (pressed) =>
        {
            expanded = pressed;
            content.Visible = expanded;
            updateHeader(title, level);
        };
        return group;
    }

    public static PanelContainer DrawHeaderContainer(string title)
    {
        var headerContainer = new PanelContainer();
        headerContainer.AddThemeStyleboxOverride("panel", EditorThemeUtility.GetStylebox("bg", "EditorInspectorCategory"));
        {
            var hbox = new HBoxContainer();
            hbox.Alignment = BoxContainer.AlignmentMode.Center;
            {
                var titleIcon = new TextureRect();
                titleIcon.Texture = EditorThemeUtility.GetEditorIcon("Node");
                titleIcon.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
                hbox.AddChild(titleIcon);
            }
            {
                var label = new Label
                {
                    Text = title,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    LabelSettings = new LabelSettings
                    {
                        Font = EditorThemeUtility.GetFont("bold", "EditorFonts"),
                        FontSize = EditorThemeUtility.GetFontSize("bold_size", "EditorFonts"),
                        FontColor = EditorThemeUtility.GetColor("font_color", "Tree"),
                    }
                };
                hbox.AddChild(label);
            }
            headerContainer.AddChild(hbox);
        }
        return headerContainer;
    }

    public static Button DrawButton(string text, Texture2D icon = null, Action onPressed = null)
    {
        var addComponent = new Button();
        addComponent.SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter;
        addComponent.Text = text;
        if (icon != null)
        {
            addComponent.Icon = EditorThemeUtility.GetEditorIcon("Add");
        }
        if (onPressed != null)
        {
            addComponent.Pressed += onPressed;
        }
        return addComponent;
    }
}
#endif
