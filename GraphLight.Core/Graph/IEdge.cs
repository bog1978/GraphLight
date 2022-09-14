using System;
using System.Collections.Generic;
using GraphLight.Geometry;

namespace GraphLight.Graph
{
    public interface IEdge : IEdge<IVertexData, IEdgeData>
    {
        IList<Point2D> Points { get; }
        void RaisePointsChanged();
        IDisposable DeferRefresh();
    }
}