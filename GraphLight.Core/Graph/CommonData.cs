namespace GraphLight.Graph
{
    public class CommonData : ICommonData
    {
        public CommonData()
        {
            StrokeThickness = 1.0;
            FontSize = 12;
        }

        public string Label { get; set; }

        public bool IsSelected { get; set; }

        public bool IsHighlighted { get; set; }

        public int ZIndex { get; set; }

        public string Stroke { get; set; }

        public double StrokeThickness { get; set; }

        public double FontSize { get; set; }

        public string Category { get; set; }
    }
}