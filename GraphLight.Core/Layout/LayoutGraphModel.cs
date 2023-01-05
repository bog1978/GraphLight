using GraphLight.Graph;

namespace GraphLight.Layout
{
    public class LayoutGraphModel : BaseGraph<IVertexData, IEdgeData>, IGraph
    {
        public double Width { get; set; }

        public double Height { get; set; }

        protected override IVertex<IVertexData, IEdgeData> CreateVertex(IVertexData data) => new Vertex(data);

        protected override IEdge<IVertexData, IEdgeData> CreateEdge(IEdgeData data) => new Edge(data);

        public override IVertexData CreateVertexData(object id) => new VertexData(id.ToString());

        public override IEdgeData CreateEdgeData() => new EdgeData();
    }
}