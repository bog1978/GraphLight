using System;
using GraphLight.Model;

namespace GraphLight.Algorithm
{
    public interface IDepthFirstSearch<V, E> : IAlgorithm
    {
        Action<IEdge<V, E>> OnBackEdge { get; set; }
        Action<IEdge<V, E>> OnCrossEdge { get; set; }
        Action<IEdge<V, E>> OnForwardEdge { get; set; }
        Action<IVertex<V, E>> OnNode { get; set; }
        Action<IEdge<V, E>> OnTreeEdge { get; set; }
        Action<IEdge<V, E>, DfsEdgeType> OnEdge { get; set; }
    }

    public enum DfsEdgeType
    {
        Forward,
        Cross,
        Back,
        Tree,
    }
}