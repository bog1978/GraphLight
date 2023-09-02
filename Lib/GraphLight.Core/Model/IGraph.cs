using System;
using System.Collections.Generic;

namespace GraphLight.Model
{
    public interface IGraph<out G, V, E> : IElement<G>
    where V : IEquatable<V>
    {
        IReadOnlyList<IEdge<V, E>> Edges { get; }
        IReadOnlyList<V> Vertices { get; }
        IEdge<V, E> AddEdge(V srcData, V dstData, E data);
        void AddVertex(V data);
        void InsertVertex(IEdge<V, E> edge, V vertexData, E edgeData);
        void RemoveEdge(IEdge<V, E> edge);
        void RemoveVertex(V vertex);
        IReadOnlyList<IEdge<V, E>> GetEdges(V vertex);
        IReadOnlyList<IEdge<V, E>> GetInEdges(V vertex);
        IReadOnlyList<IEdge<V, E>> GetOutEdges(V vertex);
        IReadOnlyList<IEdge<V, E>> GetLoopEdges(V vertex);
        void Revert(IEdge<V, E> edge);
        void ChangeSource(IEdge<V, E> edge, V vertex);
        void ChangeDestination(IEdge<V, E> edge, V vertex);
    }
}