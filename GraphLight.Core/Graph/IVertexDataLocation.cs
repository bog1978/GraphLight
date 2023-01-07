namespace GraphLight.Graph
{
    public interface IVertexDataLocation
    {
        bool IsTmp { get; set; }
        double Width { get; set; }
        double Height { get; set; }
        double Left { get; set; }
        double Top { get; set; }
        double Right { get; }
        double Bottom { get; }
    }
}