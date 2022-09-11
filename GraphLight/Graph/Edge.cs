using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using GraphLight.Geometry;
using GraphLight.Layout;

namespace GraphLight.Graph
{
    public class Edge<TVertex, TEdge> : BaseEdge<TVertex, TEdge>, IEdge
    {
        private string _category;
        private string _color;
        private IList<Point2D> _draggablePoints;
        private bool _isHighlighted;
        private bool _isSelected;
        private IList<Point2D> _points;
        private double _thickness;
        private int _zIndex;

        public Edge()
        {
            Color = "Black";
            Thickness = 1;

            var points = new ObservableCollection<Point2D>();
            Points = points;
            points.CollectionChanged += pointsCollectionChanged;

            var draggablePoints = new ObservableCollection<Point2D>();
            DraggablePoints = draggablePoints;
        }

        public Edge(TEdge data) : this()
        {
            Data = data;
        }

        public string StrokeBrush => Color;

        int IEdge.DstPointIndex { get; set; }

        IList<Point2D> IEdge.PolygonPoints { get; set; }

        public IList<Point2D> Points
        {
            get => _points;
            set => SetProperty(ref _points, value);
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                SetProperty(ref _isSelected, value);
                IsHighlighted = value;
            }
        }

        public bool IsHighlighted
        {
            get => _isHighlighted;
            set => SetProperty(ref _isHighlighted, value);
        }

        public IList<Point2D> DraggablePoints
        {
            get => _draggablePoints;
            set => SetProperty(ref _draggablePoints, value);
        }

        public double Length
        {
            get
            {
                var vector = ((IVertex)Src).CenterPoint() - ((IVertex)Dst).CenterPoint();
                return vector.Len * Weight;
            }
        }

        public string Color
        {
            get => _color;
            set
            {
                SetProperty(ref _color, value);
                RaisePropertyChanged(nameof(StrokeBrush));
            }
        }

        public double Thickness
        {
            get => _thickness;
            set => SetProperty(ref _thickness, value);
        }

        public int ZIndex
        {
            get => _zIndex;
            set => SetProperty(ref _zIndex, value);
        }

        public string Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
        }

        IVertex IEdge.Src
        {
            get => (IVertex)Src;
            set => Src = (IVertex<TVertex, TEdge>)value;
        }

        IVertex IEdge.Dst
        {
            get => (IVertex)Dst;
            set => Dst = (IVertex<TVertex, TEdge>)value;
        }

        object IElement.Data => Data;

        public void Revert()
        {
            if (IsRevert)
                throw new Exception("Edge is already reverted.");
            var tmp = Src;
            Src = Dst;
            Dst = tmp;
            IsRevert = true;
        }

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
            FixDraggablePoints(data);
        }

        public void FixDraggablePoints(Point2D data)
        {
            var j = DraggablePoints.IndexOf(data);
            var p3 = DraggablePoints[j];
            if (j > 1)
            {
                var p1 = DraggablePoints[j - 2];
                var p2 = DraggablePoints[j - 1];
                p2.X = (p1.X + p3.X) / 2;
                p2.Y = (p1.Y + p3.Y) / 2;
            }
            if (j < DraggablePoints.Count - 2)
            {
                var p4 = DraggablePoints[j + 1];
                var p5 = DraggablePoints[j + 2];
                p4.X = (p3.X + p5.X) / 2;
                p4.Y = (p3.Y + p5.Y) / 2;
            }
        }

        public void UpdateSrcPort()
        {
            if (Points.Count < 2)
                return;
            if (IsRevert)
            {
                var p1 = Points[Points.Count - 1];
                var p2 = Points[Points.Count - 2];
                var p = ((IVertex)Src).GetShapePort(p2);
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
                var p = ((IVertex)Src).GetShapePort(p2);
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
                var p = ((IVertex)Dst).GetShapePort(p2);
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
                var p = ((IVertex)Dst).GetShapePort(p2);
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
                        DraggablePoints.Insert(0, Points[0]);
                    }
                    else if (e.NewStartingIndex == Points.Count - 1)
                    {
                        // Last point.
                        p1 = DraggablePoints[i - 2];
                        p3 = Points[e.NewStartingIndex];
                        p2 = p1 + (p3 - p1) / 2;
                        DraggablePoints.Add(p2);
                        DraggablePoints.Add(p3);
                    }
                    else
                    {
                        // Middle point
                        p1 = DraggablePoints[i - 2];
                        p3 = DraggablePoints[i - 1];
                        p5 = DraggablePoints[i];
                        p2 = p1 + (p3 - p1) / 2;
                        p4 = p3 + (p5 - p3) / 2;
                        DraggablePoints.Insert(i - 1, p2);
                        DraggablePoints.Insert(i + 1, p4);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    i = 2 * e.OldStartingIndex;
                    // Сначала фиксим порты, чтобы правильно вычислить координаты средней точки.
                    UpdateSrcPort();
                    UpdateDstPort();
                    p1 = DraggablePoints[i - 2];
                    p2 = DraggablePoints[i - 1];
                    p3 = DraggablePoints[i];
                    p4 = DraggablePoints[i + 1];
                    p5 = DraggablePoints[i + 2];
                    DraggablePoints.Remove(p2);
                    DraggablePoints.Remove(p4);
                    p3.X = (p1.X + p5.X) / 2;
                    p3.Y = (p1.Y + p5.Y) / 2;
                    RaisePointsChanged();
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    DraggablePoints.Clear();
                    foreach (var point in Points)
                        DraggablePoints.Add(point);
                    break;
            }
        }

        private class RefreshHelper : IDisposable
        {
            private readonly Edge<TVertex, TEdge> _edge;

            internal RefreshHelper(Edge<TVertex, TEdge> edge) => _edge = edge;

            #region IDisposable Members

            public void Dispose() => _edge.RaisePointsChanged();

            #endregion
        }
    }
}