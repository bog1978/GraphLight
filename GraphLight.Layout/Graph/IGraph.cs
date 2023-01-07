namespace GraphLight.Graph
{
    public interface IGraph : IGraph<IVertexData, IEdgeData>
    {
        double Width { get; set; }
        double Height { get; set; }
    }
}