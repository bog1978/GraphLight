using System;
using System.Collections.Generic;
using GraphLight.Geometry;
using GraphLight.ViewModel;

namespace GraphLight.Graph
{
    public class Edge<TVertex, TEdge> : BaseViewModel//, IEdge<TVertex, TEdge>
    {
        private TEdge _data;
        private Vertex<TVertex, TEdge> _dst, _src;
        private double _weight = 1;
        private bool _isRevert;
        private int _dstPointIndex;
        private IList<Point2D> _polygonPoints;
        private IList<Point2D> _points;
        private bool _isSelected;
        private bool _isHighlighted;

        public Edge(TEdge data)
        {
            _data = data;
        }

        #region IEdge<TVertex,TEdge> Members

        public TEdge Data
        {
            get { return _data; }
            set { SetProperty(ref _data, value, "Data"); }
        }

        public double Weight
        {
            get { return _weight; }
            set { SetProperty(ref _weight, value, "Weight"); }
        }

        public bool IsRevert
        {
            get { return _isRevert; }
            set { SetProperty(ref _isRevert, value, "IsRevert"); }
        }

        public int DstPointIndex
        {
            get { return _dstPointIndex; }
            set { SetProperty(ref _dstPointIndex, value, "DstPointIndex"); }
        }

        public IList<Point2D> PolygonPoints
        {
            get { return _polygonPoints; }
            set { SetProperty(ref _polygonPoints, value, "PolygonPoints"); }
        }

        public IList<Point2D> Points
        {
            get { return _points; }
            set { SetProperty(ref _points, value, "Points"); }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                SetProperty(ref _isSelected, value, "IsSelected");
                IsHighlighted = value;
            }
        }

        public bool IsHighlighted
        {
            get { return _isHighlighted; }
            set { SetProperty(ref _isHighlighted, value, "IsHighlighted"); }
        }


        public Vertex<TVertex, TEdge> Src
        {
            get { return _src; }
            set
            {
                var oldValue = _src;
                if (oldValue == value)
                    return;
                _src = value;
                if (oldValue != null)
                    oldValue.RegisterEdge(this);
                if (value != null)
                    value.RegisterEdge(this);
                RaisePropertyChanged("Src");
            }
        }

        public Vertex<TVertex, TEdge> Dst
        {
            get { return _dst; }
            set
            {
                var oldValue = _dst;
                if (oldValue == value)
                    return;
                _dst = value;
                if (oldValue != null)
                    oldValue.RegisterEdge(this);
                if (value != null)
                    value.RegisterEdge(this);
                RaisePropertyChanged("Dst");
            }
        }

        public void Revert()
        {
            if (IsRevert)
                throw new Exception("Edge is already reverted.");
            var tmp = Src;
            Src = Dst;
            Dst = tmp;
            IsRevert = true;
        }

        #endregion
    }
}