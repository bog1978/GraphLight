namespace GraphLight.Graph
{
    public abstract class CommonData : ICommonData
    {
        protected CommonData(string? label, string? category)
        {
            Label = label;
            Category = category;
            StrokeThickness = 1.0;
            FontSize = 12;
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
        public StrokeStyle StrokeStyle { get; set; }
        public string Background { get; set; }
        public string Foreground { get; set; }
        public FontStyle FontStyle { get; set; }
        public FontWeight FontWeight { get; set; }
        public TextAlignment TextAlignment { get; set; }
        public TextWrapping TextWrapping { get; set; }
    }
}