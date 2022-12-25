using GraphLight.Geometry;
using System.Collections.Generic;

namespace GraphLight.Graph
{
    public interface IEdgeData : ICommonData
    {
        string Color { get; set; }
        double Thickness { get; set; }
        IList<Point2D> Points { get; }
    }
}