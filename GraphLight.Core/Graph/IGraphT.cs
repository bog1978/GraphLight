using System.Collections.Generic;

namespace GraphLight.Graph
{
    public interface IGraph<V, E>
    {
        IVertex<V, E> this[V key] { get; }
        IEnumerable<IEdge<V, E>> Edges { get; }
        IEnumerable<IVertex<V, E>> Vertices { get; }
        IEnumerable<object> All { get; }
        IEdge<V, E> AddEdge(IVertex<V, E> src, IVertex<V, E> dst, E data);
        IEdge<V, E> AddEdge(V srcData, V dstData, E data);
        IVertex<V, E> AddVertex(V data);
        IVertex<V, E> InsertVertex(IEdge<V, E> edge, V vertexData);
        void RemoveEdge(IEdge<V, E> edge);
        void RemoveVertex(IVertex<V, E> vertex);
    }
}