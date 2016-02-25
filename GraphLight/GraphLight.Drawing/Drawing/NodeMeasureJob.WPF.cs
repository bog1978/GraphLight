using System.Windows;
using System.Windows.Controls;
using GraphLight.Graph;
using GraphLight.Layout;

namespace GraphLight.Drawing
{
    public class NodeMeasure : INodeMeasure<VertexAttrs, EdgeAttrs>
    {
        private const double MARGIN = 20;

        private static readonly Size _maxSize = new Size(
            double.PositiveInfinity, double.PositiveInfinity);

        public void Measure(IVertex<VertexAttrs, EdgeAttrs> vertex)
        {
            MeasureStatic(vertex);
        }

        public static void MeasureStatic(IVertex<VertexAttrs, EdgeAttrs> attrs)
        {
            var textBlock = new TextBlock { Text = attrs.Data.Label };
            textBlock.Measure(_maxSize);
            attrs.Data.Width = attrs.Data.IsTmp ? MARGIN / 2 : textBlock.DesiredSize.Width + MARGIN;
            attrs.Data.Height = attrs.Data.IsTmp ? MARGIN / 2 : textBlock.DesiredSize.Height + MARGIN;
        }
    }
}