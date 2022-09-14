using GraphLight.Geometry;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GraphLight.Graph
{
    public class EdgeData : CommonData, IEdgeData
    {
        private readonly IList<Point2D> _draggablePoints;
        private readonly IList<Point2D> _points;

        private double _thickness;
        private string _color;

        public EdgeData()
        {
            Color = "Black";
            Thickness = 1;
            _points = new ObservableCollection<Point2D>();
            _draggablePoints = new ObservableCollection<Point2D>();
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

        public string StrokeBrush => Color;

        IList<Point2D> IEdgeData.Points => _points;
        IList<Point2D> IEdgeData.PolygonPoints { get; set; }
        int IEdgeData.DstPointIndex { get; set; }
        public IList<Point2D> DraggablePoints => _draggablePoints;

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
    }
}