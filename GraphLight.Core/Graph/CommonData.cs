namespace GraphLight.Graph
{
    public class CommonData : ICommonData
    {
        public CommonData()
        {
            StrokeThickness = 1.0;
            FontSize = 12;
            Style = StrokeStyle.Solid;
            Background = "White";
            Foreground = "Black";
            Stroke = "Black";
        }

        public string? Category { get; set; }

        public string? Label { get; set; }

        public bool IsSelected { get; set; }

        public bool IsHighlighted { get; set; }

        public int ZIndex { get; set; }

        public string Stroke { get; set; }

        public double StrokeThickness { get; set; }

        public double FontSize { get; set; }

        public StrokeStyle Style { get; set; }

        public string Background { get; set; }

        public string Foreground { get; set; }
    }
}