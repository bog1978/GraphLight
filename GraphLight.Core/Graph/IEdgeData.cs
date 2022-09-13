namespace GraphLight.Graph
{
    public interface IEdgeData
    {
        bool IsSelected { get; set; }
        bool IsHighlighted { get; set; }
        int ZIndex { get; set; }
        string Category { get; set; }
        string Color { get; set; }
        double Thickness { get; set; }
    }
}