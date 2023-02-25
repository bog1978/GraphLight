using System;
using GraphLight.Model;

namespace GraphLight.Algorithm
{
    public interface IDepthFirstSearch<V, E> : IAlgorithm
        where V : IEquatable<V>
    {
        Action<IVertexInfo<V>>? OnNode { get; set; }
        Action<IEdgeInfo<V, E>>? OnEdge { get; set; }
    }

    public interface IEdgeInfo<V, E>
        where V : IEquatable<V>
    {
        IEdge<V, E> Edge { get; }
        DfsEdgeType EdgeType { get; }
        int Order { get; }
    }

    public interface IVertexInfo<V>
        where V : IEquatable<V>
    {
        V Vertex { get; }
        int Order { get; }
        int Depth { get; }
        DfsVertexType VertexType { get; }
    }

    public enum DfsEdgeType
    {
        Forward,
        Cross,
        Back,
        Tree,
    }

    public enum DfsVertexType
    {
        Root,
        Middle,
        MiddleCycle,
        Leaf,
        LeafCycle,
    }
}