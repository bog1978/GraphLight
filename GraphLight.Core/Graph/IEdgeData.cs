using GraphLight.Geometry;
using System.Collections.Generic;

namespace GraphLight.Graph
{
    public interface IEdgeData : ICommonData
    {
        IList<Point2D> Points { get; }
    }
}