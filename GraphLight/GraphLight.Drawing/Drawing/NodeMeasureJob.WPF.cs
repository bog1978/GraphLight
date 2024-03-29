﻿using System.Windows;
using System.Windows.Controls;
using GraphLight.Graph;
using GraphLight.Layout;

namespace GraphLight.Drawing
{
    public class NodeMeasure : INodeMeasure
    {
        private const double MARGIN = 20;

        private static readonly Size _maxSize = new Size(
            double.PositiveInfinity, double.PositiveInfinity);

        public void Measure(IVertex vertex)
        {
            MeasureStatic(vertex);
        }

        public static void MeasureStatic(IVertex attrs)
        {
            var textBlock = new TextBlock { Text = attrs.Label };
            textBlock.Measure(_maxSize);
            attrs.Width = attrs.IsTmp ? MARGIN / 2 : textBlock.DesiredSize.Width + MARGIN;
            attrs.Height = attrs.IsTmp ? MARGIN / 2 : textBlock.DesiredSize.Height + MARGIN;
        }
    }
}