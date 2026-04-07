// Basic contract for any object that can be selected
public interface ISelectable
{
    void OnSelected();
    void OnDeselected();
    string GetDisplayName();
}