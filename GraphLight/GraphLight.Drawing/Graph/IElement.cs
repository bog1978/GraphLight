namespace GraphLight.Graph
{
    public interface IElement
    {
        object Data { get; }
        bool IsSelected { get; set; }
        bool IsHighlighted { get; set; }
        int ZIndex { get; set; }
        string Category { get; set; }
    }
}