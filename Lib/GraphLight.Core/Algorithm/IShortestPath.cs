using System;
using GraphLight.Model;

namespace GraphLight.Algorithm
{
    public interface IShortestPath<V, E>
        where V : IEquatable<V>
        where E : notnull
    {
        void Execute(V start, V end);
        Action<IEdge<V, E>> EnterEdge { get; set; }
        Action<V> EnterNode { get; set; }
    }
}
