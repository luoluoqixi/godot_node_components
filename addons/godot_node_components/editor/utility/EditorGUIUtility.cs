#if TOOLS
using System;
using Godot;

namespace GodotNodeComponents.Editor;

internal static class EditorGUIUtility
{
    public static Window OpenPopupWindow(string title, Vector2I size, Action<VBoxContainer> drawContent)
    {
        var popup = new Window();
        float scaleFactor = DisplayServer.ScreenGetDpi() / 96.0f;
        size = new Vector2I((int)(size.X * scaleFactor), (int)(size.Y * scaleFactor));
        popup.InitialPosition = Window.WindowInitialPosition.CenterPrimaryScreen;
        popup.Size = size;
        popup.Exclusive = false;
        popup.Borderless = false;
        popup.TransparentBg = false;
        popup.Title = title;
        popup.Theme = EditorInterface.Singleton.GetBaseControl().Theme;
        {
            var container = new PanelContainer();
            container.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            container.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
            container.SetAnchorsPreset(Control.LayoutPreset.FullRect);
            container.AddThemeStyleboxOverride("panel", EditorThemeUtility.GetBgStyleboxFlat("base_color", "Editor"));
            {
                var marginContainer = new MarginContainer();
                int marginValue = 10;
                marginContainer.AddThemeConstantOverride("margin_top", marginValue);
                marginContainer.AddThemeConstantOverride("margin_left", marginValue);
                marginContainer.AddThemeConstantOverride("margin_bottom", marginValue);
                marginContainer.AddThemeConstantOverride("margin_right", marginValue);
                {
                    var vbox = new VBoxContainer();
                    drawContent(vbox);
                    marginContainer.AddChild(vbox);
                }
                container.AddChild(marginContainer);
            }
            popup.AddChild(container);
        }
        popup.CloseRequested += () =>
        {
            popup.QueueFree();
        };

        EditorInterface.Singleton.PopupDialog(popup);
        return popup;
    }

    public static VBoxContainer DrawCollapsibleGroup(string title, int level = 1, PopupMenu contextMenu = null, string savePath = null)
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
        var boldSize = EditorThemeUtility.GetFontSize("bold_size", "EditorFonts");
        headerButton.AddThemeFontSizeOverride("font_size", boldSize);
        var styleBox = new StyleBoxFlat();
        headerButton.AddThemeStyleboxOverride("normal", styleBox);

        headerButton.Icon = expanded
            ? EditorThemeUtility.GetIcon("arrow", "Tree")
            : EditorThemeUtility.GetIcon("arrow_collapsed", "Tree");

        string indent = new string(' ', (level - 1) * 2);
        headerButton.Text = $"{indent}{title}";

        group.AddChild(headerButton);

        var content = new VBoxContainer();
        content.Visible = expanded;
        group.AddChild(content);

        headerButton.Toggled += (pressed) =>
        {
            if (content == null) return;
            expanded = pressed;
            content.Visible = expanded;
            headerButton.Icon = expanded
            ? EditorThemeUtility.GetIcon("arrow", "Tree")
            : EditorThemeUtility.GetIcon("arrow_collapsed", "Tree");
        };

        if (contextMenu != null)
        {
            headerButton.GuiInput += (e) =>
            {
                if (e is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Right && mouseEvent.Pressed)
                {
                    contextMenu.Position = (Vector2I)(headerButton.GetScreenPosition() + headerButton.GetLocalMousePosition());
                    contextMenu.Popup();
                }
            };

            headerButton.AddChild(contextMenu);
        }
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

    public static Button DrawButton(string text, Texture2D icon = null, Action onPressed = null, bool expandHorizontal = false)
    {
        var addComponent = new Button();
        if (!expandHorizontal)
        {
            addComponent.SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter;
        }
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

    public static UndoRedo GetUndoRedo()
    {
        return NodeComponentsPlugin.UndoRedo;
    }
    public static void MarkSceneAsUnsaved()
    {
        EditorInterface.Singleton.MarkSceneAsUnsaved();
    }
}
#endif
