using System.Collections.Generic;
using System.Linq;

namespace GraphLight.Graph
{
    public abstract class GraphModelBase<TVertex, TEdge> : BaseGraph<Vertex<TVertex, TEdge>, Edge<TVertex, TEdge>, TVertex, TEdge>, IGraph
    {
        private double _width;
        private double _height;

        public IEnumerable<IElement> Elements
        {
            get { return Verteces.OfType<IElement>().Union(Edges); }
        }

        public double Width
        {
            get { return _width; }
            set { SetProperty(ref _width, value, "Width"); }
        }

        public double Height
        {
            get { return _height; }
            set { SetProperty(ref _height, value, "Height"); }
        }

        IEnumerable<IEdge> IGraph.Edges
        {
            get { return Edges; }
        }

        IEnumerable<IVertex> IGraph.Verteces
        {
            get { return Verteces; }
        }

        IEdge IGraph.AddEdge(object src, object dst)
        {
            return AddEdge((TVertex)src, (TVertex)dst);
        }

        IEdge IGraph.AddEdge(object src, object dst, object data)
        {
            return AddEdge((TVertex)src, (TVertex)dst, (TEdge)data);
        }

        IVertex IGraph.InsertVertex(IEdge edge)
        {
            return InsertVertex((Edge<TVertex, TEdge>)edge, CreateVertexData());
        }

        void IGraph.RemoveEdge(IEdge edge)
        {
            RemoveEdge((Edge<TVertex, TEdge>)edge);
        }

        void IGraph.RemoveVertex(IVertex vertex)
        {
            RemoveVertex((Vertex<TVertex, TEdge>)vertex);
        }

        IVertex IGraph.this[object key]
        {
            get { return this[(TVertex)key]; }
        }

        public IVertex AddVertex()
        {
            return AddVertex(CreateVertexData());
        }

        IVertex IGraph.AddVertex(object data)
        {
            return AddVertex((TVertex) data);
        }
    }
}