using System.Windows.Controls;
using GraphLight.Graph;
using GraphLight.Layout;

namespace GraphLight.Drawing
{
    public class NodeMeasure : INodeMeasure<VertexAttrs, EdgeAttrs>
    {
        private const double MARGIN = 20;

        public static void MeasureStatic(IVertex<VertexAttrs, EdgeAttrs> attrs)
        {
            var textBlock = new TextBlock { Text = attrs.Label };
            attrs.Width = attrs.IsTmp ? MARGIN / 2 : textBlock.ActualWidth + MARGIN;
            attrs.Height = attrs.IsTmp ? MARGIN / 2 : textBlock.ActualHeight + MARGIN;
        }

        public void Measure(IVertex<VertexAttrs, EdgeAttrs> vertex)
        {
            MeasureStatic(vertex);
        }
    }
}