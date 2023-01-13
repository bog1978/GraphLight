namespace GraphLight.Graph
{
    public class LayoutGraph : GenericGraph<IGraphData, IVertexData, IEdgeData>
    {
        public LayoutGraph(IGraphData data) : base(data)
        {
        }
    }
}