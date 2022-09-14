using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using GraphLight.Geometry;
using GraphLight.Layout;

namespace GraphLight.Graph
{
    public class Edge : BaseEdge<IVertexData, IEdgeData>, IEdge
    {
        private readonly IList<Point2D> _points;

        public Edge(IEdgeData data) : base(data)
        {
            var points = new ObservableCollection<Point2D>();
            points.CollectionChanged += pointsCollectionChanged;
            _points = points;
        }

        public IList<Point2D> Points => _points;

        public void UpdatePoint(Point2D data)
        {
            var i = Points.IndexOf(data);
            if (i == 1 && !IsRevert)
                UpdateSrcPort();
            else if (i == 1 && IsRevert)
                UpdateDstPort();
            if (i == Points.Count - 2 && !IsRevert)
                UpdateDstPort();
            else if (i == Points.Count - 2 && IsRevert)
                UpdateSrcPort();
            Data.FixDraggablePoints(data);
        }

        public void UpdateSrcPort()
        {
            if (Points.Count < 2)
                return;
            if (IsRevert)
            {
                var p1 = Points[Points.Count - 1];
                var p2 = Points[Points.Count - 2];
                var p = Src.GetShapePort(p2);
                if (p1 != p)
                {
                    p1.X = p.X;
                    p1.Y = p.Y;
                }
            }
            else
            {
                var p1 = Points[0];
                var p2 = Points[1];
                var p = Src.GetShapePort(p2);
                if (p1 != p)
                {
                    p1.X = p.X;
                    p1.Y = p.Y;
                }
            }
        }

        public void UpdateDstPort()
        {
            if (Points.Count < 2)
                return;
            if (IsRevert)
            {
                var p1 = Points[0];
                var p2 = Points[1];
                var p = Dst.GetShapePort(p2);
                if (p1 != p)
                {
                    p1.X = p.X;
                    p1.Y = p.Y;
                }
            }
            else
            {
                var p1 = Points[Points.Count - 1];
                var p2 = Points[Points.Count - 2];
                var p = Dst.GetShapePort(p2);
                if (p1 != p)
                {
                    p1.X = p.X;
                    p1.Y = p.Y;
                }
            }
        }

        public void RaisePointsChanged() => RaisePropertyChanged(nameof(Points));

        public IDisposable DeferRefresh() => new RefreshHelper(this);

        protected void pointsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Point2D p1, p2, p3, p4, p5;
            int i;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    i = 2 * e.NewStartingIndex;
                    if (e.NewStartingIndex == 0)
                    {
                        // First point. Do nothing.
                        Data.DraggablePoints.Insert(0, Points[0]);
                    }
                    else if (e.NewStartingIndex == Points.Count - 1)
                    {
                        // Last point.
                        p1 = Data.DraggablePoints[i - 2];
                        p3 = Points[e.NewStartingIndex];
                        p2 = p1 + (p3 - p1) / 2;
                        Data.DraggablePoints.Add(p2);
                        Data.DraggablePoints.Add(p3);
                    }
                    else
                    {
                        // Middle point
                        p1 = Data.DraggablePoints[i - 2];
                        p3 = Data.DraggablePoints[i - 1];
                        p5 = Data.DraggablePoints[i];
                        p2 = p1 + (p3 - p1) / 2;
                        p4 = p3 + (p5 - p3) / 2;
                        Data.DraggablePoints.Insert(i - 1, p2);
                        Data.DraggablePoints.Insert(i + 1, p4);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    i = 2 * e.OldStartingIndex;
                    // Сначала фиксим порты, чтобы правильно вычислить координаты средней точки.
                    UpdateSrcPort();
                    UpdateDstPort();
                    p1 = Data.DraggablePoints[i - 2];
                    p2 = Data.DraggablePoints[i - 1];
                    p3 = Data.DraggablePoints[i];
                    p4 = Data.DraggablePoints[i + 1];
                    p5 = Data.DraggablePoints[i + 2];
                    Data.DraggablePoints.Remove(p2);
                    Data.DraggablePoints.Remove(p4);
                    p3.X = (p1.X + p5.X) / 2;
                    p3.Y = (p1.Y + p5.Y) / 2;
                    RaisePointsChanged();
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Data.DraggablePoints.Clear();
                    foreach (var point in Points)
                        Data.DraggablePoints.Add(point);
                    break;
            }
        }

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