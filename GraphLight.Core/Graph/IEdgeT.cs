using System;

namespace GraphLight.Graph
{
    public interface IEdge<TVertexData, TEdgeData>
    {
        double Weight { get; set; }
        TEdgeData Data { get; set; }
        IVertex<TVertexData, TEdgeData> Dst { get; set; }
        IVertex<TVertexData, TEdgeData> Src { get; set; }
        bool IsRevert { get; set; }

        event EventHandler<EdgeChangedEventArgs<TVertexData, TEdgeData>> EdgeChanged;
    }
}