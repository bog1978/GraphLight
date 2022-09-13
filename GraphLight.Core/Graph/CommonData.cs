namespace GraphLight.Graph
{
    public class CommonData : BaseViewModel, ICommonData
    {
        private string _category;
        private bool _isHighlighted;
        private bool _isSelected;
        private int _zIndex;

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
    }
}