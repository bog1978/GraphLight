using System;
using System.Collections.Generic;
using GraphLight.Geometry;

namespace GraphLight.Graph
{
    public interface IEdge : IEdge<IVertexData, IEdgeData>
    {
        int DstPointIndex { get; set; }
        IList<Point2D> PolygonPoints { get; set; }
        IList<Point2D> Points { get; }
        IList<Point2D> DraggablePoints { get; }
        void UpdatePoint(Point2D data);
        void FixDraggablePoints(Point2D data);
        void UpdateSrcPort();
        void UpdateDstPort();
        void RaisePointsChanged();
        IDisposable DeferRefresh();
    }
}