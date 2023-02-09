using System;
using GraphLight.Model;

namespace GraphLight.Algorithm
{
    public interface IDepthFirstSearch<V, E> : IAlgorithm
    {
        Action<IVertexInfo<V>>? OnNode { get; set; }
        Action<IEdgeInfo<V, E>>? OnEdge { get; set; }
    }

    public interface IEdgeInfo<V, E>
    {
        IEdge<V, E> Edge { get; }
        DfsEdgeType EdgeType { get; }
        int Order { get; }
    }

    public interface IVertexInfo<V>
    {
        V Vertex { get; }
        int Order { get; }
        int Depth { get; }
    }

    public enum DfsEdgeType
    {
        Forward,
        Cross,
        Back,
        Tree,
    }
}