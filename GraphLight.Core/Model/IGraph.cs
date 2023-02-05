using System.Collections.Generic;

namespace GraphLight.Model
{
    public interface IGraph<out G, V, E> : IElement<G>
    {
        IVertex<V> this[V key] { get; }
        IReadOnlyList<IEdge<V, E>> Edges { get; }
        IReadOnlyList<IVertex<V>> Vertices { get; }
        IEdge<V, E> AddEdge(V srcData, V dstData, E data);
        IVertex<V> AddVertex(V data);
        IVertex<V> InsertVertex(IEdge<V, E> edge, V vertexData, E edgeData);
        void RemoveEdge(IEdge<V, E> edge);
        void RemoveVertex(IVertex<V> vertex);
        IReadOnlyList<IEdge<V, E>> GetEdges(IVertex<V> vertex);
        IReadOnlyList<IEdge<V, E>> GetInEdges(IVertex<V> vertex);
        IReadOnlyList<IEdge<V, E>> GetOutEdges(IVertex<V> vertex);
        IReadOnlyList<IEdge<V, E>> GetLoopEdges(IVertex<V> vertex);
        void Revert(IEdge<V, E> edge);
        void ChangeSource(IEdge<V, E> edge, IVertex<V> vertex);
        void ChangeDestination(IEdge<V, E> edge, IVertex<V> vertex);
    }
}