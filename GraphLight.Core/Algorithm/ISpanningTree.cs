using System;
using GraphLight.Model;

namespace GraphLight.Algorithm
{
    public interface ISpanningTree<V, E>
    {
        Action<IEdge<V, E>> EnterEdge { get; set; }

        void Execute(IVertex<V> root);
    }
}