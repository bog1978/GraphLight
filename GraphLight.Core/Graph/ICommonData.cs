namespace GraphLight.Graph
{
    public interface ICommonData
    {
        string Category { get; set; }
        string Label { get; set; }
        bool IsHighlighted { get; set; }
        bool IsSelected { get; set; }
        int ZIndex { get; set; }
        string Stroke { get; set; }
        double StrokeThickness { get; set; }
        double FontSize { get; set; }
        StrokeStyle Style { get; set; }
    }
}