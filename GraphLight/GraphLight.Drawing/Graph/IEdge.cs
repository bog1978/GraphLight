using System.Collections.Generic;
using GraphLight.Geometry;

namespace GraphLight.Graph
{
    public interface IEdge<TVertex, TEdge>
    {
        TEdge Data { get; set; }
        IVertex<TVertex, TEdge> Src { get; set; }
        IVertex<TVertex, TEdge> Dst { get; set; }
        double Weight { get; set; }
        bool IsRevert { get; set; }
        int DstPointIndex { get; set; }
        IList<Point2D> PolygonPoints { get; set; }
        IList<Point2D> Points { get; set; }
        bool IsSelected { get; set; }
        bool IsHighlighted { get; set; }
        void Revert();
    }
}