using GraphLight.Graph;

namespace GraphLight.Layout
{
    public class LayoutGraphModel : BaseGraph<IVertexData, IEdgeData>, IGraph
    {
        private double _width;
        private double _height;

        public double Width
        {
            get => _width;
            set => SetProperty(ref _width, value);
        }

        public double Height
        {
            get => _height;
            set => SetProperty(ref _height, value);
        }

        protected override IVertex<IVertexData, IEdgeData> CreateVertex(IVertexData data) => new Vertex(data);

        protected override IEdge<IVertexData, IEdgeData> CreateEdge(IEdgeData data) => new Edge(data);

        public override IVertexData CreateVertexData(object id) => new VertexData(id.ToString());

        public override IEdgeData CreateEdgeData() => new EdgeData();
    }
}