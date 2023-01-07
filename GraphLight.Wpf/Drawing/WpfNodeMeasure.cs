using System.Windows;
using System.Windows.Controls;
using GraphLight.Algorithm;
using GraphLight.Graph;

namespace GraphLight.Drawing
{
    public class WpfNodeMeasure<V, E> : INodeMeasure<V, E>
        where V : IVertexData
    {
        private const double MARGIN = 20;

        private static readonly Size _maxSize = new Size(
            double.PositiveInfinity, double.PositiveInfinity);

        public void Measure(IVertex<V, E> vertex)
        {
            var textBlock = new TextBlock { Text = vertex.Data.Label };
            textBlock.Measure(_maxSize);
            vertex.Data.Rect.Width = vertex.Data.IsTmp ? MARGIN / 2 : textBlock.DesiredSize.Width + MARGIN;
            vertex.Data.Rect.Height = vertex.Data.IsTmp ? MARGIN / 2 : textBlock.DesiredSize.Height + MARGIN;
        }
    }
}