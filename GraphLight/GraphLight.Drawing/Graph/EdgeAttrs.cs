using System.Collections.Generic;
using System.Diagnostics;
using GraphLight.Geometry;
using GraphLight.ViewModel;

namespace GraphLight.Graph
{
    [DebuggerDisplay("IsRevert={IsRevert}, CutValue={CutValue}")]
    public class EdgeAttrs : BaseViewModel, IEdgeAttrs
    {
        private string _color;
        private IList<Point2D> _draggablePoints;
        private int _dstPointIndex;
        private bool _isHighlighted;
        private bool _isRevert;
        private bool _isSelected;
        private IList<Point2D> _points;
        private IList<Point2D> _polygonPoints;
        private double _thickness;
        private int _zIndex;
        
        public string Color
        {
            get { return _color; }
            set
            {
                _color = value;
                RaisePropertyChanged("Color");
            }
        }

        public double Thickness
        {
            get { return _thickness; }
            set
            {
                _thickness = value;
                RaisePropertyChanged("Thickness");
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                RaisePropertyChanged("IsSelected");
                IsHighlighted = value;
            }
        }

        public bool IsHighlighted
        {
            get { return _isHighlighted; }
            set
            {
                _isHighlighted = value;
                RaisePropertyChanged("IsHighlighted");
            }
        }

        public int ZIndex
        {
            get { return _zIndex; }
            set
            {
                _zIndex = value;
                RaisePropertyChanged("ZIndex");
            }
        }

        public IList<Point2D> DraggablePoints
        {
            get { return _draggablePoints; }
            set
            {
                _draggablePoints = value;
                RaisePropertyChanged("DraggablePoints");
            }
        }

        #region IEdgeAttrs Members

        public double CutValue { get; set; }

        public bool IsRevert
        {
            get { return _isRevert; }
            set
            {
                _isRevert = value;
                RaisePropertyChanged("IsRevert");
            }
        }

        public int DstPointIndex
        {
            get { return _dstPointIndex; }
            set
            {
                _dstPointIndex = value;
                RaisePropertyChanged("DstPointIndex");
            }
        }

        public IList<Point2D> PolygonPoints
        {
            get { return _polygonPoints; }
            set
            {
                _polygonPoints = value;
                RaisePropertyChanged("PolygonPoints");
            }
        }

        public IList<Point2D> Points
        {
            get { return _points; }
            set
            {
                _points = value;
                RaisePropertyChanged("Points");
            }
        }

        #endregion
    }
}