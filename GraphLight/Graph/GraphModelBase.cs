using System.Collections.Generic;
using System.Linq;

namespace GraphLight.Graph
{
    public abstract class GraphModelBase<TVertex, TEdge> : BaseGraph<TVertex, TEdge>, IGraph
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

        IEdge IGraph.AddEdge(object src, object dst, object data) => (IEdge)AddEdge((TVertex)src, (TVertex)dst, (TEdge)data);

        IVertex IGraph.InsertVertex(IEdge edge, object data) => (IVertex)InsertVertex((Edge<TVertex, TEdge>)edge, (TVertex)data);

        void IGraph.RemoveEdge(IEdge edge) => RemoveEdge((Edge<TVertex, TEdge>)edge);

        void IGraph.RemoveVertex(IVertex vertex) => RemoveVertex((Vertex<TVertex, TEdge>)vertex);

        IVertex IGraph.this[object key] => (IVertex)this[(TVertex)key];

        IVertex IGraph.AddVertex(object data) => (IVertex)AddVertex((TVertex) data);
    }
}