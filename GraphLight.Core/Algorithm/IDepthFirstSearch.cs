using System;
using GraphLight.Model;

namespace GraphLight.Algorithm
{
    public interface IDepthFirstSearch<V, E> : IAlgorithm
    {
        Action<IVertexInfo<V, E>>? OnNode { get; set; }
        Action<IEdgeInfo<V, E>>? OnEdge { get; set; }
    }

    public interface IEdgeInfo<V, E>
    {
        IEdge<V, E> Edge { get; }
        DfsEdgeType EdgeType { get; }
        int Order { get; }
    }

    public interface IVertexInfo<V, E>
    {
        IVertex<V, E> Vertex { get; }
        int Order { get; }
    }

    public enum DfsEdgeType
    {
        Forward,
        Cross,
        Back,
        Tree,
    }
}