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