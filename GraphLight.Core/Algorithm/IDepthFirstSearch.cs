using System;
using GraphLight.Model;

namespace GraphLight.Algorithm
{
    public interface IDepthFirstSearch<V, E> : IAlgorithm
    {
        Action<IVertex<V, E>> OnNode { get; set; }
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