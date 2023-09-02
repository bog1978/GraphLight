using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphLight.Geometry
{
    /// <summary>
    ///This is a test class for Polygon2DTest and is intended
    ///to contain all Polygon2DTest Unit Tests
    ///</summary>
    [TestClass]
    public class Polygon2DTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes

        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //

        #endregion

        /// <summary>
        ///A test for IsInner
        ///</summary>
        [TestMethod]
        public void IsInnerPointTest()
        {
            var points = new List<Point2D>
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(0, 1),
                };
            var target = new Polygon2D(points);
            var result = target.IsInner(new Point2D(0.5, 0.5));
            Assert.IsTrue(result);
        }

        /// <summary>
        ///A test for IsInner
        ///</summary>
        [TestMethod]
        public void IsOuterPointTest()
        {
            var points = new List<Point2D>
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(0, 1),
                };
            var target = new Polygon2D(points);
            var result = target.IsInner(new Point2D(5, 0.5));
            Assert.IsFalse(result);
        }

        /// <summary>
        ///A test for IsConvex
        ///</summary>
        [TestMethod]
        public void IsConvexTest()
        {
            var points = new List<Point2D>
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(0, 1),
                };
            var target = new Polygon2D(points);
            var result = target.IsConvex();
            Assert.IsTrue(result);
        }

        /// <summary>
        ///A test for IsConvex
        ///</summary>
        [TestMethod]
        public void IsConcaveTest()
        {
            var points = new List<Point2D>
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(0, 1),
                    new Point2D(0.5, 0.5),
                };
            var target = new Polygon2D(points);
            var result = target.IsConvex();
            Assert.IsFalse(result);
        }

        /// <summary>
        ///A test for Polygon2D Constructor
        ///</summary>
        [TestMethod]
        public void Polygon2DConstructorTest()
        {
            var points = new List<Point2D>
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(0, 1),
                };
            var target = new Polygon2D(points);
            var result = target.Points.Select((p, i) => p == points[i]).All(x => x);
            Assert.IsTrue(result);
        }

        /// <summary>
        ///A test for IsInner
        ///</summary>
        [TestMethod]
        public void IsInnerTest()
        {
            var polygon = getTestPolygon();
            Assert.IsTrue(polygon.IsInner(new Point2D(1, 2), new Point2D(3, 2)));
            Assert.IsTrue(polygon.IsInner(new Point2D(1, 1), new Point2D(3, 1)));
            Assert.IsFalse(polygon.IsInner(new Point2D(1, 0), new Point2D(3, 0)));
            Assert.IsFalse(polygon.IsInner(new Point2D(0, 0), new Point2D(2, 0)));
            Assert.IsFalse(polygon.IsInner(new Point2D(0, 0), new Point2D(4, 0)));
            Assert.IsFalse(polygon.IsInner(new Point2D(-1, 1), new Point2D(5, 1)));
            Assert.IsTrue(polygon.IsInner(new Point2D(1, 1), new Point2D(2, 3)));
            Assert.IsFalse(polygon.IsInner(new Point2D(0, 3), new Point2D(2, 2)));
            Assert.IsFalse(polygon.IsInner(new Point2D(0, 5), new Point2D(2, 5)));

            // Линии частично или полноостью проходят по ребрам полигона.
            // Пока считаем такие линии не принадлежащими полигону.
            Assert.IsFalse(polygon.IsInner(new Point2D(0, 2), new Point2D(2, 3)));
            Assert.IsFalse(polygon.IsInner(new Point2D(0, 2), new Point2D(4, 0)));
        }

        /// <summary>
        ///A test for Split
        ///</summary>
        [TestMethod]
        public void SplitTest()
        {
            var polygon = getTestPolygon();
            var pol1 = polygon.Split(3, 7);
            // TODO: добавить проверки.
        }

        private static Polygon2D getTestPolygon()
        {
            var points = new[]
                {
                    new Point2D(0, 0),
                    new Point2D(0, 2),
                    new Point2D(2, 3),
                    new Point2D(4, 2),
                    new Point2D(4, 0),
                    new Point2D(3, 1),
                    new Point2D(2, 0),
                    new Point2D(1, 1),
                };
            return new Polygon2D(points);
        }
    }
}