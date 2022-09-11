﻿using System.Windows;
using System.Windows.Controls;
using GraphLight.Graph;
using GraphLight.Layout;

namespace GraphLight.Drawing
{
    public class NodeMeasure<V, E> : INodeMeasure<V, E>
    {
        private const double MARGIN = 20;

        private static readonly Size _maxSize = new Size(
            double.PositiveInfinity, double.PositiveInfinity);

        public void Measure(IVertex<V, E> vertex)
        {
            MeasureStatic(vertex);
        }

        public static void MeasureStatic(IVertex<V, E> vertex)
        {
            var v = (IVertex)vertex;
            var textBlock = new TextBlock { Text = v.Label };
            textBlock.Measure(_maxSize);
            v.Width = v.IsTmp ? MARGIN / 2 : textBlock.DesiredSize.Width + MARGIN;
            v.Height = v.IsTmp ? MARGIN / 2 : textBlock.DesiredSize.Height + MARGIN;
        }
    }
}