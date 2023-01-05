namespace GraphLight.Graph
{
    public class CommonData : BaseViewModel, ICommonData
    {
        private string _label;
        private string _category;
        private bool _isHighlighted;
        private bool _isSelected;
        private int _zIndex;
        private string _stroke;
        private double _strokeThickness;
        private double _fontSize;

        public CommonData()
        {
            StrokeThickness = 1.0;
            FontSize = 12;
        }

        public string Label
        {
            get => _label;
            set => SetProperty(ref _label, value);
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

        public string Stroke
        {
            get => _stroke;
            set => SetProperty(ref _stroke, value);
        }

        public double StrokeThickness
        {
            get => _strokeThickness;
            set => SetProperty(ref _strokeThickness, value);
        }

        public double FontSize
        {
            get => _fontSize;
            set => SetProperty(ref _fontSize, value);
        }

        public string Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
        }
    }
}