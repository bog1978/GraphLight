using System.Drawing;

namespace GraphLight.Graph
{
    public class EdgeData : BaseViewModel, IEdgeData
    {
        private string _category;
        private bool _isHighlighted;
        private bool _isSelected;
        private int _zIndex;
        private double _thickness;
        private string _color;

        public EdgeData()
        {
            Color = "Black";
            Thickness = 1;
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
    }
}