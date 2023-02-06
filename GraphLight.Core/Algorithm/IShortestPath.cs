using System;
using GraphLight.Model;

namespace GraphLight.Algorithm
{
    public interface IShortestPath<V, E>
    {
        void Execute(V start, V end);
        Action<IEdge<V, E>> EnterEdge { get; set; }
        Action<V> EnterNode { get; set; }
    }
}
