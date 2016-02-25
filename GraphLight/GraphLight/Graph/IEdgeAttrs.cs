using System.Collections.Generic;
using GraphLight.Geometry;

namespace GraphLight.Graph
{
    public interface IEdgeAttrs
    {
        double CutValue { get; set; }
        bool IsRevert { get; set; }
        int DstPointIndex { get; set; }
        IList<Point2D> PolygonPoints { get; set; }
        IList<Point2D> Points { get; set; }
    }
}