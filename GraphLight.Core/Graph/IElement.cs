namespace GraphLight.Graph
{
    public interface IElement
    {
        bool IsSelected { get; set; }
        bool IsHighlighted { get; set; }
        int ZIndex { get; set; }
        string Category { get; set; }
    }
}