using System.Collections.Generic;
using System.Linq;

namespace GraphLight.Graph
{
    public class LayoutGraphModel : BaseGraph<IVertexData, IEdgeData>, IGraph
    {
        private double _width;
        private double _height;

        public IEnumerable<IElement> Elements => Enumerable.Union(
            Vertices.Cast<IElement>(),
            Edges.Cast<IElement>());

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

        IEdge IGraph.AddEdge(object src, object dst, object data) => (IEdge)AddEdge((IVertexData)src, (IVertexData)dst, (IEdgeData)data);

        IVertex IGraph.InsertVertex(IEdge edge, object data) => (IVertex)InsertVertex((Edge)edge, (IVertexData)data);

        void IGraph.RemoveEdge(IEdge edge) => RemoveEdge((Edge)edge);

        void IGraph.RemoveVertex(IVertex vertex) => RemoveVertex((Vertex)vertex);

        IVertex IGraph.this[object key] => (IVertex)this[(IVertexData)key];

        IVertex IGraph.AddVertex(object data) => (IVertex)AddVertex((IVertexData) data);

        protected override IVertex<IVertexData, IEdgeData> CreateVertex(IVertexData data) => new Vertex(data);

        protected override IEdge<IVertexData, IEdgeData> CreateEdge(IEdgeData data) => new Edge(data);
    }
}