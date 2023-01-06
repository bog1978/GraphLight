using System.Windows;
using System.Windows.Controls;
using GraphLight.Graph;

namespace GraphLight.Drawing
{
    public class EdgeControl : Control
    {
        public EdgeControl() => DefaultStyleKey = typeof (EdgeControl);

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
            nameof(Data), typeof(IEdgeData), typeof(EdgeControl), new PropertyMetadata(default(IEdgeData?)));

        public IEdgeData? Data
        {
            get => (IEdgeData?)GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }
    }
}