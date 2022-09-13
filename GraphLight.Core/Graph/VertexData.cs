using System;

namespace GraphLight.Graph
{
    public class VertexData : BaseViewModel, IVertexData
    {
        private double _width;
        private double _height;
        private double _left;
        private double _top;
        private string _category;
        private bool _isHighlighted;
        private bool _isSelected;
        private string _label;
        private string _shapeData;
        private int _zIndex;

        public VertexData(string id)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            ShapeData = "M 0,1 A 1,1 0 1 0 2,1 A 1,1 0 1 0 0,1";
        }

        public string Id { get; }

        bool IVertexDataLocation.IsTmp { get; set; }

        int IVertexDataLayered.Rank { get; set; }
        int IVertexDataLayered.Position { get; set; }

        double IVertexDataLocation.CenterX { get; set; }

        public double Width
        {
            get => _width;
            set
            {
                SetProperty(ref _width, value);
                RaisePropertyChanged(nameof(Right));
            }
        }

        public double Height
        {
            get => _height;
            set
            {
                SetProperty(ref _height, value);
                RaisePropertyChanged(nameof(Bottom));
            }
        }

        public double Left
        {
            get => _left;
            set
            {
                SetProperty(ref _left, value);
                RaisePropertyChanged(nameof(Right));
            }
        }

        public double Top
        {
            get => _top;
            set
            {
                SetProperty(ref _top, value);
                RaisePropertyChanged(nameof(Bottom));
            }
        }

        public double Right => Left + Width;

        public double Bottom => Top + Height;

        public int ZIndex
        {
            get => _zIndex;
            set => SetProperty(ref _zIndex, value);
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

        public string Label
        {
            get => _label;
            set => SetProperty(ref _label, value);
        }

        public string ShapeData
        {
            get => _shapeData;
            set => SetProperty(ref _shapeData, value);
        }

        public string Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
        }

        public bool Equals(IVertexData other) => other?.Id == Id;

        public int CompareTo(IVertexData other) => StringComparer.Ordinal.Compare(Id, other?.Id);

        public override int GetHashCode() => Id.GetHashCode();

        public override bool Equals(object obj) => Equals(obj as IVertexData);

        public override string ToString() => Id;
    }
}