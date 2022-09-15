using System;
using GraphLight.Graph;

namespace GraphLight.Algorithm
{
    public interface IShortestPath<V, E>
    {
        void Execute(V start, V end);
        Action<IEdge<V, E>> EnterEdge { get; set; }
        Action<IVertex<V, E>> EnterNode { get; set; }
    }
}
