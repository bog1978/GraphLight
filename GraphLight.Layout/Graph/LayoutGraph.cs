namespace GraphLight.Graph
{
    public class LayoutGraph : GenericGraph<IGraphData, IVertexData, IEdgeData>
    {
        public LayoutGraph(IGraphData data) : base(data)
        {
        }

        public override IVertexData CreateVertexData(object id) => new VertexData(id.ToString(), null, null);

        public override IEdgeData CreateEdgeData() => new EdgeData(null, null);
    }
}