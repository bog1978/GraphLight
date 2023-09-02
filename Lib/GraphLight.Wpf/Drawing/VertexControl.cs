using System;
using System.Windows;
using System.Windows.Controls;
using GraphLight.Model;

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

        protected override Size MeasureOverride(Size constraint)
        {
            var size = base.MeasureOverride(constraint);
            var width = size.Width;
            var height = size.Height;
            var sqrt2 = Math.Sqrt(2);

            // Подгоняем размер так, чтобы площадь описанной фигуры была минимальна,
            // а содержимое в неё полностью помещалось.
            var shapeSize = Data switch
            {
                { Shape: VertexShape.Diamond } => new Size(width * 2, height * 2),
                { Shape: VertexShape.Ellipse } => new Size(width * sqrt2, height * sqrt2),
                _ => size,
            };

            return Data != null
                ? new Size(shapeSize.Width + Data.Margin * 2, shapeSize.Height + Data.Margin * 2)
                : shapeSize;
        }
    }
}