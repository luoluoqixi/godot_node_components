namespace GodotNodeComponents;

public interface IComponents
{
    public ComponentsController Components { get; set; }
    public void SaveComponents(string[] data);
    public string[] LoadComponents();
}
