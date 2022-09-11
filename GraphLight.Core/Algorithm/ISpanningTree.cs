using System;
using GraphLight.Graph;

namespace GraphLight.Algorithm
{
    public interface ISpanningTree<V, E>
    {
        Action<IEdge<V, E>> EnterEdge { get; set; }

        void Execute(IVertex<V, E> root);
    }
}