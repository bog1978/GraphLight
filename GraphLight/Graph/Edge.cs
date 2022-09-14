using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using GraphLight.Geometry;

namespace GraphLight.Graph
{
    public class Edge : BaseEdge<IVertexData, IEdgeData>, IEdge
    {
        private readonly IList<Point2D> _points;

        public Edge(IEdgeData data) : base(data)
        {
            var points = (ObservableCollection<Point2D>)data.Points;
            points.CollectionChanged += pointsCollectionChanged;
            _points = points;
        }

        public IList<Point2D> Points => _points;

        public void RaisePointsChanged() => RaisePropertyChanged(nameof(Points));

        public IDisposable DeferRefresh() => new RefreshHelper(this);

        protected void pointsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) =>
            this.HandlePointsCollectionChanged(e);

        private class RefreshHelper : IDisposable
        {
            private readonly Edge _edge;

            internal RefreshHelper(Edge edge) => _edge = edge;

            #region IDisposable Members

            public void Dispose() => _edge.RaisePointsChanged();

            #endregion
        }
    }
}