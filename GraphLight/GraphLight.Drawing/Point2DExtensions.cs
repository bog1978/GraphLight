using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using GraphLight.Geometry;

namespace GraphLight
{
    public static class Point2DExtensions
    {
        public static Point ToPoint(this Point2D point2D)
        {
            return new Point(point2D.X, point2D.Y);
        }
    }
}
