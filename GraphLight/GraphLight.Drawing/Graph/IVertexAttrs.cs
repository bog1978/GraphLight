namespace GraphLight.Graph
{
    public interface IVertexAttrs
    {
        int Rank { get; set; }
        int Position { get; set; }
        bool IsTmp { get; set; }
        string Id { get; set; }
        string Label { get; set; }
        double Width { get; set; }
        double Height { get; set; }
        double Left { get; set; }
        double Top { get; set; }
        double Right { get; }
        double Bottom { get; }
        double CenterX { get; set; }
        VertexColor Color { get; set; }
        string Category { get; set; }
        IVertexAttrs Clone();
    }
}