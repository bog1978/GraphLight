using System.Collections.Generic;
using GraphLight.Geometry;

namespace GraphLight.Model
{
    public interface IEdgeData :
        ICommonData,
        IEdgeDataWeight
    {
        IList<Point2D> Points { get; }
    }
}