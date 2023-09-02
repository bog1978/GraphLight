using System.Windows;
using System.Windows.Controls;
using GraphLight.Algorithm;
using GraphLight.Model;

namespace GraphLight.Drawing
{
    public class WpfNodeMeasure: INodeMeasure
    {
        private const double MARGIN = 20;

        private static readonly Size _maxSize = new Size(
            double.PositiveInfinity, double.PositiveInfinity);

        public void Measure(IVertexData vertex)
        {
            var textBlock = new TextBlock { Text = vertex.Label };
            textBlock.Measure(_maxSize);
            vertex.Rect.Width = vertex.IsTmp ? MARGIN / 2 : textBlock.DesiredSize.Width + MARGIN;
            vertex.Rect.Height = vertex.IsTmp ? MARGIN / 2 : textBlock.DesiredSize.Height + MARGIN;
        }
    }
}