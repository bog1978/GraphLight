using System;
using GraphLight.Graph;

namespace GraphLight.Algorithm
{
    public interface IDepthFirstSearch<V, E>
    {
        Action<IEdge<V, E>> OnBackEdge { get; set; }
        Action<IEdge<V, E>> OnCrossEdge { get; set; }
        Action<IEdge<V, E>> OnForwardEdge { get; set; }
        Action<IVertex<V, E>> OnNode { get; set; }
        Action<IEdge<V, E>> OnTreeEdge { get; set; }

        void Execute();
    }
}