using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Media;
using GraphLight.Geometry;
using GraphLight.Layout;

namespace GraphLight.Graph
{
    [DebuggerDisplay("{DebugString}")]
    public class DrawingEdge : Edge<VertexAttrs, EdgeAttrs>, INotifyPropertyChanged
    {
        public DrawingEdge(EdgeAttrs data)
            : base(data)
        {
            base.Data = data;
            Color = Colors.Black;
            Thickness = 1;

            var points = new ObservableCollection<Point2D>();
            base.Data.Points = points;
            points.CollectionChanged += pointsCollectionChanged;

            var draggablePoints = new ObservableCollection<Point2D>();
            base.Data.DraggablePoints = draggablePoints;
        }

        public string DebugString
        {
            get
            {
                var format = !Data.IsRevert ? "{0} -> {1}" : "{1} <- {0}";
                return string.Format(format, Src != null ? Src.Id : "?", Dst != null ? Dst.Id : "?");
            }
        }

        public IEnumerable<Point2D> DraggablePoints
        {
            get { return Data.DraggablePoints; }
        }

        public double Lenght
        {
            get
            {
                var vector = Src.Data.CenterPoint() - Dst.Data.CenterPoint();
                return vector.Len*Weight;
            }
        }

        public Color Color
        {
            get { return Data.Color; }
            set
            {
                Data.Color = value;
                RaisePropertyChanged("Color");
                RaisePropertyChanged("StrokeBrush");
            }
        }

        public Brush StrokeBrush
        {
            get { return new SolidColorBrush(Color); }
        }

        public double Thickness
        {
            get { return Data.Thickness; }
            set
            {
                Data.Thickness = value;
                RaisePropertyChanged("Thickness");
            }
        }

        public override EdgeAttrs Data
        {
            get { return base.Data; }
            set
            {
                base.Data = value;
                RaisePropertyChanged("Data");
            }
        }

        public override double Weight
        {
            get { return base.Weight; }
            set
            {
                base.Weight = value;
                RaisePropertyChanged("Weight");
            }
        }

        public override IVertex<VertexAttrs, EdgeAttrs> Src
        {
            get { return base.Src; }
            set
            {
                var oldValue = Src;
                if (oldValue == value)
                    return;
                base.Src = value;
                if (oldValue != null)
                    oldValue.RegisterEdge(this);
                if (value != null)
                    value.RegisterEdge(this);
                RaisePropertyChanged("Src");
            }
        }

        public override IVertex<VertexAttrs, EdgeAttrs> Dst
        {
            get { return base.Dst; }
            set
            {
                var oldValue = Dst;
                if (oldValue == value)
                    return;
                base.Dst = value;
                if (oldValue != null)
                    oldValue.RegisterEdge(this);
                if (value != null)
                    value.RegisterEdge(this);
                RaisePropertyChanged("Dst");
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public override void Revert()
        {
            if (Data.IsRevert)
                throw new Exception("Edge is already reverted.");
            base.Revert();
            Data.IsRevert = true;
        }

        private void pointsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Point2D p1, p2, p3, p4, p5;
            int i;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    i = 2*e.NewStartingIndex;
                    if (e.NewStartingIndex == 0)
                    {
                        // First point. Do nothing.
                        Data.DraggablePoints.Insert(0, Data.Points[0]);
                    }
                    else if (e.NewStartingIndex == Data.Points.Count - 1)
                    {
                        // Last point.
                        p1 = Data.DraggablePoints[i - 2];
                        p3 = Data.Points[e.NewStartingIndex];
                        p2 = p1 + (p3 - p1)/2;
                        Data.DraggablePoints.Add(p2);
                        Data.DraggablePoints.Add(p3);
                    }
                    else
                    {
                        // Middle point
                        p1 = Data.DraggablePoints[i - 2];
                        p3 = Data.DraggablePoints[i - 1];
                        p5 = Data.DraggablePoints[i];
                        p2 = p1 + (p3 - p1)/2;
                        p4 = p3 + (p5 - p3)/2;
                        Data.DraggablePoints.Insert(i - 1, p2);
                        Data.DraggablePoints.Insert(i + 1, p4);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    i = 2*e.OldStartingIndex;
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
                    p3.X = (p1.X + p5.X)/2;
                    p3.Y = (p1.Y + p5.Y)/2;
                    RaisePointsChanged();
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Data.DraggablePoints.Clear();
                    foreach (var point in Data.Points)
                        Data.DraggablePoints.Add(point);
                    break;
            }
        }

        public void UpdatePoint(Point2D data)
        {
            var i = Data.Points.IndexOf(data);
            if (i == 1 && !Data.IsRevert)
                UpdateSrcPort();
            else if (i == 1 && Data.IsRevert)
                UpdateDstPort();
            if (i == Data.Points.Count - 2 && !Data.IsRevert)
                UpdateDstPort();
            else if (i == Data.Points.Count - 2 && Data.IsRevert)
                UpdateSrcPort();
            FixDraggablePoints(data);
        }

        public void FixDraggablePoints(Point2D data)
        {
            var j = Data.DraggablePoints.IndexOf(data);
            var p3 = Data.DraggablePoints[j];
            if (j > 1)
            {
                var p1 = Data.DraggablePoints[j - 2];
                var p2 = Data.DraggablePoints[j - 1];
                p2.X = (p1.X + p3.X)/2;
                p2.Y = (p1.Y + p3.Y)/2;
            }
            if (j < Data.DraggablePoints.Count - 2)
            {
                var p4 = Data.DraggablePoints[j + 1];
                var p5 = Data.DraggablePoints[j + 2];
                p4.X = (p3.X + p5.X)/2;
                p4.Y = (p3.Y + p5.Y)/2;
            }
        }

        public void UpdateSrcPort()
        {
            if (Data.Points.Count < 2)
                return;
            if (Data.IsRevert)
            {
                var p1 = Data.Points[Data.Points.Count - 1];
                var p2 = Data.Points[Data.Points.Count - 2];
                var p = Src.Data.GetShapePort(p2);
                if (p1 != p)
                {
                    p1.X = p.X;
                    p1.Y = p.Y;
                }
            }
            else
            {
                var p1 = Data.Points[0];
                var p2 = Data.Points[1];
                var p = Src.Data.GetShapePort(p2);
                if (p1 != p)
                {
                    p1.X = p.X;
                    p1.Y = p.Y;
                }
            }
        }

        public void UpdateDstPort()
        {
            if (Data.Points.Count < 2)
                return;
            if (Data.IsRevert)
            {
                var p1 = Data.Points[0];
                var p2 = Data.Points[1];
                var p = Dst.Data.GetShapePort(p2);
                if (p1 != p)
                {
                    p1.X = p.X;
                    p1.Y = p.Y;
                }
            }
            else
            {
                var p1 = Data.Points[Data.Points.Count - 1];
                var p2 = Data.Points[Data.Points.Count - 2];
                var p = Dst.Data.GetShapePort(p2);
                if (p1 != p)
                {
                    p1.X = p.X;
                    p1.Y = p.Y;
                }
            }
        }

        public void RaisePointsChanged()
        {
            RaisePropertyChanged("Data");
        }

        public IDisposable DeferRefresh()
        {
            return new RefreshHelper(this);
        }

        #region Nested type: RefreshHelper

        private class RefreshHelper : IDisposable
        {
            private readonly DrawingEdge _edge;

            internal RefreshHelper(DrawingEdge edge)
            {
                _edge = edge;
            }

            #region IDisposable Members

            public void Dispose()
            {
                _edge.RaisePointsChanged();
            }

            #endregion
        }

        #endregion
    }
}