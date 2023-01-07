namespace GraphLight.Graph
{
    public class LayoutGraph : BaseGraph<IVertexData, IEdgeData>, IGraph
    {
        public double Width { get; set; }

        public double Height { get; set; }

        protected override IVertex<IVertexData, IEdgeData> CreateVertex(IVertexData data) => new LayoutVertex(data);

        protected override IEdge<IVertexData, IEdgeData> CreateEdge(IEdgeData data) => new LayoutEdge(data);

        public override IVertexData CreateVertexData(object id) => new VertexData(id.ToString(), null, null);

        public override IEdgeData CreateEdgeData() => new EdgeData(null, null);
    }
}