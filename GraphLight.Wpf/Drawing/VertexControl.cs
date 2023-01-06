using System.Windows;
using System.Windows.Controls;
using GraphLight.Graph;

namespace GraphLight.Drawing
{
    public class VertexControl : Control
    {
        public VertexControl() => DefaultStyleKey = typeof(VertexControl);

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
            nameof(Data), typeof(IVertexData), typeof(VertexControl), new PropertyMetadata(default(IVertexData?)));

        public IVertexData? Data
        {
            get => (IVertexData?)GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }
    }
}