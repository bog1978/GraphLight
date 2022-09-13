namespace GraphLight.Graph
{
    public interface ICommonData
    {
        string Category { get; set; }
        bool IsHighlighted { get; set; }
        bool IsSelected { get; set; }
        int ZIndex { get; set; }
    }
}