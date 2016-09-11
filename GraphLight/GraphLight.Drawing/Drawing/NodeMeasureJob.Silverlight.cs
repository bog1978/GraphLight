using System.Windows.Controls;
using GraphLight.Graph;
using GraphLight.Layout;

namespace GraphLight.Drawing
{
    public class NodeMeasure : INodeMeasure
    {
        private const double MARGIN = 20;

        public static void MeasureStatic(IVertex attrs)
        {
            var textBlock = new TextBlock { Text = attrs.Label };
            attrs.Width = attrs.IsTmp ? MARGIN / 2 : textBlock.ActualWidth + MARGIN;
            attrs.Height = attrs.IsTmp ? MARGIN / 2 : textBlock.ActualHeight + MARGIN;
        }

        public void Measure(IVertex vertex)
        {
            MeasureStatic(vertex);
        }
    }
}