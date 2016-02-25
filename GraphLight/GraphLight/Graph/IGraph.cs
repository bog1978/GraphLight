using System.Collections.Generic;

namespace GraphLight.Graph
{
    public interface IGraph<TVertex, TEdge>
    {
        double Width { get; set; }
        double Height { get; set; }
        IEnumerable<IVertex<TVertex, TEdge>> Verteces { get; }
        IEnumerable<IEdge<TVertex, TEdge>> Edges { get; }
        IVertex<TVertex, TEdge> this[TVertex key] { get; }
        IEdge<TVertex, TEdge> AddEdge(TVertex src, TVertex dst, TEdge data);
        IEdge<TVertex, TEdge> AddEdge(IVertex<TVertex, TEdge> src, IVertex<TVertex, TEdge> dst, TEdge data);
        IVertex<TVertex, TEdge> AddVertex(TVertex data);
        void RemoveEdge(IEdge<TVertex, TEdge> edge);
        void RemoveVertex(IVertex<TVertex, TEdge> vertex);
        IVertex<TVertex, TEdge> InsertVertex(IEdge<TVertex, TEdge> edge, TVertex vertexData);
    }
}
