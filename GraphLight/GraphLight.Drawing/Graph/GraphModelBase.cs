using System.Collections.Generic;
using System.Linq;

namespace GraphLight.Graph
{
    public abstract class GraphModelBase<TVertex, TEdge> : BaseGraph<Vertex<TVertex, TEdge>, Edge<TVertex, TEdge>, TVertex, TEdge>, IGraph
    {
        private double _width;
        private double _height;

        public IEnumerable<IElement> Elements => Vertices.OfType<IElement>().Union(Edges);

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

        IEnumerable<IEdge> IGraph.Edges => Edges;

        IEnumerable<IVertex> IGraph.Vertices => Vertices;

        IEdge IGraph.AddEdge(object src, object dst) => AddEdge((TVertex)src, (TVertex)dst);

        IEdge IGraph.AddEdge(object src, object dst, object data) => AddEdge((TVertex)src, (TVertex)dst, (TEdge)data);

        IVertex IGraph.InsertVertex(IEdge edge) => InsertVertex((Edge<TVertex, TEdge>)edge, CreateVertexData());

        void IGraph.RemoveEdge(IEdge edge) => RemoveEdge((Edge<TVertex, TEdge>)edge);

        void IGraph.RemoveVertex(IVertex vertex) => RemoveVertex((Vertex<TVertex, TEdge>)vertex);

        IVertex IGraph.this[object key] => this[(TVertex)key];

        public IVertex AddVertex() => AddVertex(CreateVertexData());

        IVertex IGraph.AddVertex(object data) => AddVertex((TVertex) data);
    }
}