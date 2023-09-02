using System;
using GraphLight.Model;

namespace GraphLight.Algorithm
{
    public interface ISpanningTree<V, E>
        where V : IEquatable<V>
    {
        Action<IEdge<V, E>> EnterEdge { get; set; }

        void Execute(V root);
    }
}