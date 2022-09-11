using System;

namespace GraphLight.Graph
{
    public abstract partial class BaseGraph<TVertex, TEdge, TVertexData, TEdgeData> where TVertex : BaseGraph<TVertex, TEdge, TVertexData, TEdgeData>.Vertex, new()
        where TEdge : BaseGraph<TVertex, TEdge, TVertexData, TEdgeData>.Edge, new()
    {
        internal class EdgeChangedEventArgs : EventArgs
        {
            public EdgeChangedEventArgs(TVertex oldVertex, TVertex newVertex)
            {
                OldVertex = oldVertex;
                NewVertex = newVertex;
            }

            public TVertex OldVertex { get; }
            public TVertex NewVertex { get; }
        }
    }
}