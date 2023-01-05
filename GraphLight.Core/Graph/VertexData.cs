using System;

namespace GraphLight.Graph
{
    public class VertexData : CommonData, IVertexData
    {
        private double _width;
        private double _height;
        private double _left;
        private double _top;
        private VertexShape _shape;
        private string _background;
        private string _foreground;

        public VertexData(string id)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Label = id;
            Shape = VertexShape.Ellipse;
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

        public VertexShape Shape
        {
            get => _shape;
            set => SetProperty(ref _shape, value);
        }

        public string Background
        {
            get => _background;
            set => SetProperty(ref _background, value);
        }

        public string Foreground
        {
            get => _foreground;
            set => SetProperty(ref _foreground, value);
        }

        public bool Equals(IVertexData other) => other?.Id == Id;

        public int CompareTo(IVertexData other) => StringComparer.Ordinal.Compare(Id, other?.Id);

        public override int GetHashCode() => Id.GetHashCode();

        public override bool Equals(object obj) => Equals(obj as IVertexData);

        public override string ToString() => Id;
    }
}