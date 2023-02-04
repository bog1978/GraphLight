using System.Collections.Generic;

namespace GraphLight.Model
{
    public interface IGraph<out G, V, E> : IElement<G>
    {
        IVertex<V, E> this[V key] { get; }
        IReadOnlyList<IEdge<V, E>> Edges { get; }
        IReadOnlyList<IVertex<V, E>> Vertices { get; }
        IEdge<V, E> AddEdge(V srcData, V dstData, E data);
        IVertex<V, E> AddVertex(V data);
        IVertex<V, E> InsertVertex(IEdge<V, E> edge, V vertexData, E edgeData);
        void RemoveEdge(IEdge<V, E> edge);
        void RemoveVertex(IVertex<V, E> vertex);
        IReadOnlyList<IEdge<V, E>> GetEdges(IVertex<V, E> vertex);
        IReadOnlyList<IEdge<V, E>> GetInEdges(IVertex<V, E> vertex);
        IReadOnlyList<IEdge<V, E>> GetOutEdges(IVertex<V, E> vertex);
        IReadOnlyList<IEdge<V, E>> GetLoopEdges(IVertex<V, E> vertex);
    }
}