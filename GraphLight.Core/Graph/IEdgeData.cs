namespace GraphLight.Graph
{
    public interface IEdgeData : ICommonData
    {
        string Color { get; set; }
        double Thickness { get; set; }
    }
}