using System.Collections.Generic;
using GraphLight.Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphLight.Test.Geometry
{
    [TestClass]
    public class Polygon2DSplitConvexTest
    {
        [TestMethod]
        public void Test1()
        {
            var points = new List<Point2D>
            {
                new Point2D(1421.5, 68.1111111111111),
                new Point2D(1772.5, 84.2222222222222),
                new Point2D(1772.5, 110.222222222222),
                new Point2D(1772.5, 126.333333333333),
                new Point2D(1772.5, 152.333333333333),
                new Point2D(1772.5, 168.444444444444),
                new Point2D(1772.5, 194.444444444444),
                new Point2D(1888.66666666667, 210.555555555556),
                new Point2D(1888.66666666667, 236.555555555556),
                new Point2D(1772.5, 252.666666666667),
                new Point2D(1772.5, 278.666666666667),
                new Point2D(1657.83333333333, 294.777777777778),
                new Point2D(1657.83333333333, 320.777777777778),
                new Point2D(1421.5, 336.888888888889),
                new Point2D(1421.5, 336.888888888889),
                new Point2D(1421.5, 320.777777777778),
                new Point2D(1421.5, 294.777777777778),
                new Point2D(1421.5, 278.666666666667),
                new Point2D(1543.16666666667, 278.666666666667),
                new Point2D(1657.83333333333, 278.666666666667),
                new Point2D(1657.83333333333, 252.666666666667),
                new Point2D(1543.16666666667, 252.666666666667),
                new Point2D(1543.16666666667, 236.555555555556),
                new Point2D(1657.83333333333, 236.555555555556),
                new Point2D(1657.83333333333, 210.555555555556),
                new Point2D(1657.83333333333, 194.444444444444),
                new Point2D(1657.83333333333, 168.444444444444),
                new Point2D(1543.16666666667, 152.333333333333),
                new Point2D(1543.16666666667, 126.333333333333),
                new Point2D(1543.16666666667, 110.222222222222),
                new Point2D(1543.16666666667, 84.2222222222222),
                new Point2D(1421.5, 84.2222222222222),
                new Point2D(1421.5, 68.1111111111111),
            };
            var pol = new Polygon2D(points);
            var pols = pol.SplitConvex();
        }
    }
}