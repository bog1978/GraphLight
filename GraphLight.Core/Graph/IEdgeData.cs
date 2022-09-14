using GraphLight.Geometry;
using System.Collections.Generic;

namespace GraphLight.Graph
{
    public interface IEdgeData : ICommonData
    {
        string Color { get; set; }
        double Thickness { get; set; }
        int DstPointIndex { get; set; }
        IList<Point2D> PolygonPoints { get; set; }
        IList<Point2D> DraggablePoints { get; }
        void FixDraggablePoints(Point2D data);
    }
}