namespace GraphLight.Model
{
    public interface ICommonData
    {
        string? Category { get; set; }
        string? Label { get; set; }
        bool IsHighlighted { get; set; }
        bool IsSelected { get; set; }
        int ZIndex { get; set; }
        string Stroke { get; set; }
        double StrokeThickness { get; set; }
        double FontSize { get; set; }
        StrokeStyle StrokeStyle { get; set; }
        string Background { get; set; }
        string Foreground { get; set; }
        FontStyle FontStyle { get; set; }
        FontWeight FontWeight { get; set; }
        TextAlignment TextAlignment { get; set; }
        TextWrapping TextWrapping { get; set; }
    }
}