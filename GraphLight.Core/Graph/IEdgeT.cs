using System;

namespace GraphLight.Graph
{
    public interface IEdge<TVertexData, TEdgeData>
    {
        double Weight { get; set; }
        bool IsRevert { get; }
        TEdgeData Data { get; }
        IVertex<TVertexData, TEdgeData> Dst { get; set; }
        IVertex<TVertexData, TEdgeData> Src { get; set; }

        void Revert();

        event EventHandler<EdgeChangedEventArgs<TVertexData, TEdgeData>> EdgeChanged;
    }
}