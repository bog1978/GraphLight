using System.Collections.Generic;
using System.Collections.ObjectModel;
using GraphLight.Geometry;

namespace GraphLight.Graph
{
    public class EdgeData : CommonData, IEdgeData
    {
        private double _thickness;
        private string _color;

        public EdgeData()
        {
            Color = "Black";
            Thickness = 1;
            Points = new ObservableCollection<Point2D>();
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

        public IList<Point2D> Points { get; }
    }
}