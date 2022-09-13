using System.Collections.Generic;
using System.Linq;

namespace GraphLight.Graph
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

        IEnumerable<IEdge> IGraph.Edges => Edges.Cast<IEdge>();

        IEnumerable<IVertex> IGraph.Vertices => Vertices.Cast<IVertex>();

        protected override IVertex<IVertexData, IEdgeData> CreateVertex(IVertexData data) => new Vertex(data);

        protected override IEdge<IVertexData, IEdgeData> CreateEdge(IEdgeData data) => new Edge(data);
    }
}