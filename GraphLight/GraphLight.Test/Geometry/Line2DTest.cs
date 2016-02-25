using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using GraphLight.Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphLight.Test.Geometry
{
    /// <summary>
    ///This is a test class for Line2DTest and is intended
    ///to contain all Line2DTest Unit Tests
    ///</summary>
    [TestClass]
    public class Line2DTest
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
        ///A test for Line2D Constructor
        ///</summary>
        [TestMethod]
        public void Line2DConstructorTest()
        {
            var p1 = new Point2D(0, 1);
            var p2 = new Point2D(2, 3);
            var target = new Line2D(p1, p2);
            Assert.IsTrue(target.P1 == p1 && target.P2 == p2);
        }

        /// <summary>
        ///A test for Classify
        ///</summary>
        [TestMethod]
        public void ClassifyTest()
        {
            var p1 = new Point2D(0, 1);
            var p2 = new Point2D(2, 3);
            var line = new Line2D(p1, p2);
            Assert.AreEqual(line.Classify(new Point2D(0, 1)), PointLocation.Origin);
            Assert.AreEqual(line.Classify(new Point2D(2, 3)), PointLocation.Destination);
            Assert.AreEqual(line.Classify(new Point2D(1, 2)), PointLocation.Between);
            Assert.AreEqual(line.Classify(new Point2D(-1, 0)), PointLocation.Behind);
            Assert.AreEqual(line.Classify(new Point2D(3, 4)), PointLocation.Beyond);
            Assert.AreEqual(line.Classify(new Point2D(0, 2)), PointLocation.Left);
            Assert.AreEqual(line.Classify(new Point2D(2, 0)), PointLocation.Right);
        }

        /// <summary>
        ///A test for Overlap
        ///</summary>
        [TestMethod]
        public void OverlapTest()
        {
            var p0 = new Point2D(0, 0);
            var p1 = new Point2D(1, 0);
            var p2 = new Point2D(2, 0);
            var p3 = new Point2D(3, 0);
            var l01 = new Line2D(p0, p1);
            var l02 = new Line2D(p0, p2);
            var l03 = new Line2D(p0, p3);
            var l12 = new Line2D(p1, p2);
            var l13 = new Line2D(p1, p3);
            var l23 = new Line2D(p2, p3);

            Assert.IsFalse(l01.Overlap(l23));
            Assert.IsFalse(l02.Overlap(l23));
            Assert.IsTrue(l02.Overlap(l13));
            Assert.IsTrue(l12.Overlap(l03));
            Assert.IsTrue(l03.Overlap(l12));
            Assert.IsTrue(l12.Overlap(l02));
            Assert.IsTrue(l13.Overlap(l02));
            Assert.IsFalse(l23.Overlap(l02));
            Assert.IsFalse(l23.Overlap(l01));
            Assert.IsFalse(l01.Overlap(l01));
        }

        /// <summary>
        ///A test for Len
        ///</summary>
        [TestMethod]
        public void LenTest()
        {
            var target = new Line2D(new Point2D(1, 2), new Point2D(4, -2));
            Assert.AreEqual(target.Len, 5);
        }

        #region Тест пересечений

        [TestMethod]
        public void TestEqual()
        {
            var l1 = new Line2D(0, 5, 1, 5);
            var l2 = new Line2D(1, 5, 0, 5);
            Assert.AreEqual(l1.Cross(l1), CrossType.Equal);
            Assert.AreEqual(l1.Cross(l2), CrossType.Equal);
        }

        [TestMethod]
        public void TestCross1()
        {
            var l1 = new Line2D(0, 1, 2, 1);
            var l2 = new Line2D(1, 0, 1, 2);
            Assert.AreEqual(l1.Cross(l2), CrossType.Cross);
        }

        [TestMethod]
        public void TestCross2()
        {
            var l1 = new Line2D(0, 0, 1, 1);
            var l2 = new Line2D(1, 0, 0, 1);
            Assert.AreEqual(l1.Cross(l2), CrossType.Cross);
        }

        [TestMethod]
        public void TestCross3()
        {
            var l1 = new Line2D(0, 1, 2, 1);
            var l2 = new Line2D(1, 1, 1, 2);
            Assert.AreEqual(l1.Cross(l2), CrossType.Cross);
        }

        [TestMethod]
        public void TestJoin()
        {
            var p1 = new Point2D(0, 0);
            var p2 = new Point2D(0, 5);
            var p3 = new Point2D(5, 0);
            Assert.AreEqual(Line2D.Cross(p1, p2, p1, p3), CrossType.Join);
            Assert.AreEqual(Line2D.Cross(p2, p1, p1, p3), CrossType.Join);
            Assert.AreEqual(Line2D.Cross(p1, p2, p3, p1), CrossType.Join);
            Assert.AreEqual(Line2D.Cross(p2, p1, p3, p1), CrossType.Join);
        }

        [TestMethod]
        public void TestNoneParallelH()
        {
            var p1 = new Point2D(0, 0);
            var p2 = new Point2D(5, 0);
            var p3 = new Point2D(0, 5);
            var p4 = new Point2D(5, 5);
            Assert.AreEqual(Line2D.Cross(p1, p2, p3, p4), CrossType.None);
        }

        [TestMethod]
        public void TestNoneSameLineH()
        {
            var p1 = new Point2D(0, 0);
            var p2 = new Point2D(1, 0);
            var p3 = new Point2D(3, 0);
            var p4 = new Point2D(4, 0);
            Assert.AreEqual(Line2D.Cross(p1, p2, p3, p4), CrossType.None);
        }

        [TestMethod]
        public void TestNoneParallelV()
        {
            var p1 = new Point2D(0, 0);
            var p2 = new Point2D(0, 5);
            var p3 = new Point2D(5, 0);
            var p4 = new Point2D(5, 5);
            Assert.AreEqual(Line2D.Cross(p1, p2, p3, p4), CrossType.None);
        }

        [TestMethod]
        public void TestNoneSameLineV()
        {
            var p1 = new Point2D(0, 0);
            var p2 = new Point2D(0, 1);
            var p3 = new Point2D(0, 3);
            var p4 = new Point2D(0, 4);
            Assert.AreEqual(Line2D.Cross(p1, p2, p3, p4), CrossType.None);
        }

        [TestMethod]
        public void TestNone2()
        {
            var p1 = new Point2D(0, 0);
            var p2 = new Point2D(5, 5);
            var p3 = new Point2D(2, 3);
            var p4 = new Point2D(2, 5);
            Assert.AreEqual(Line2D.Cross(p1, p2, p3, p4), CrossType.None);
        }

        [TestMethod]
        public void TestNone3()
        {
            var p1 = new Point2D(0, 0);
            var p2 = new Point2D(5, 5);
            var p3 = new Point2D(3, 2);
            var p4 = new Point2D(5, 2);
            Assert.AreEqual(Line2D.Cross(p1, p2, p3, p4), CrossType.None);
        }

        [TestMethod]
        public void TestOverlapH()
        {
            var p0 = new Point2D(0, 0);
            var p1 = new Point2D(1, 0);
            var p2 = new Point2D(2, 0);
            var p3 = new Point2D(3, 0);
            testOverlap(p0, p1, p2, p3);
        }

        [TestMethod]
        public void TestOverlapV()
        {
            var p0 = new Point2D(0, 0);
            var p1 = new Point2D(0, 1);
            var p2 = new Point2D(0, 2);
            var p3 = new Point2D(0, 3);
            testOverlap(p0, p1, p2, p3);
        }

        [TestMethod]
        public void TestOverlapD()
        {
            var p0 = new Point2D(0, 0);
            var p1 = new Point2D(1, 1);
            var p2 = new Point2D(2, 2);
            var p3 = new Point2D(3, 3);
            testOverlap(p0, p1, p2, p3);
        }

        private static void testOverlap(Point2D p0, Point2D p1, Point2D p2, Point2D p3)
        {
            Assert.AreEqual(Line2D.Cross(p0, p2, p1, p3), CrossType.Overlap);
            Assert.AreEqual(Line2D.Cross(p0, p2, p0, p1), CrossType.Overlap);
            Assert.AreEqual(Line2D.Cross(p0, p3, p1, p2), CrossType.Overlap);
            Assert.AreEqual(Line2D.Cross(p0, p2, p1, p2), CrossType.Overlap);
            Assert.AreEqual(Line2D.Cross(p0, p2, p1, p3), CrossType.Overlap);
        }

        #endregion

        /// <summary>
        ///A test for IsEmpty
        ///</summary>
        [TestMethod]
        public void IsEmptyTest()
        {
            Assert.IsTrue(Line2D.Empty.IsEmpty);
            var l1 = new Line2D(Point2D.Empty, new Point2D(1, 1));
            Assert.IsTrue(l1.IsEmpty);
            var l2 = new Line2D(new Point2D(1, 1), Point2D.Empty);
            Assert.IsTrue(l2.IsEmpty);
            var l3 = new Line2D(new Point2D(1, 1), new Point2D(2, 2));
            Assert.IsFalse(l3.IsEmpty);
        }

        [TestMethod]
        public void CompareToNull()
        {
            Line2D l1 = null;
            var r1 = l1 == null;
            Assert.IsTrue(r1);

            var l2 = new Line2D();
            var r2 = l2 == null;
            Assert.IsFalse(r2);
        }

        [TestMethod]
        public void HashCodeTest()
        {
            var l1 = new Line2D(1, 1, 2, 2);
            var l2 = new Line2D(2, 2, 1, 1);
            var l3 = new Line2D(new Point2D(1, 1), Point2D.Empty);
            var l4 = new Line2D(Point2D.Empty, new Point2D(1, 1));
            var l5 = new Line2D(null, null);

            var hc1 = l1.GetHashCode();
            var hc2 = l2.GetHashCode();
            var hc3 = l3.GetHashCode();
            var hc4 = l4.GetHashCode();
            var hc5 = l5.GetHashCode();

            Assert.AreEqual(hc1, hc2);
            Assert.AreEqual(hc3, hc4);
            Assert.IsTrue(hc5 == 0);
        }

        [TestMethod]
        public void DictionaryTest()
        {
            var l1 = new Line2D(1, 1, 2, 2);
            var l2 = new Line2D(5, 5, 2, 2);
            var l31 = new Line2D(1, 1, 2, 2);
            var l32 = new Line2D(2, 2, 1, 1);
            var l4 = new Line2D(new Point2D(1, 1), Point2D.Empty);
            //var l5 = new Line2D(Point2D.Empty, new Point2D(1, 1));
            var l6 = new Line2D(Point2D.Empty, Point2D.Empty);
            var l7 = new Line2D(null, null);
            var dict = new Dictionary<Line2D, string>();
            dict.Add(l4, "l4");
            dict.Add(l6, "l6");
            dict.Add(l7, "l7");
            dict.Add(l1, "l1");
            dict.Add(l2, "l2");
            Assert.IsTrue(dict.ContainsKey(l1));
            Assert.IsTrue(dict.ContainsKey(l2));
            Assert.IsTrue(dict.ContainsKey(l31));
            Assert.IsTrue(dict.ContainsKey(l32));
            Assert.IsTrue(dict.ContainsKey(l4));
            Assert.IsTrue(dict.ContainsKey(l6));
            Assert.IsTrue(dict.ContainsKey(l7));
        }

        [TestMethod]
        public void EqualsTest()
        {
            var l1 = new Line2D(1, 1, 2, 3);
            var l2 = new Line2D(1, 1, 2, 3);
            var l3 = new Line2D(2, 3, 1, 1);
            var l4 = new Line2D(2, 3, 5, 5);
            Assert.IsTrue(l1 == l2);
            Assert.IsTrue(l1 == l3);
            Assert.IsTrue(l1 != l4);
        }

        [TestMethod]
        public void ObjectEqualsTest()
        {
            var l1 = new Line2D(1, 1, 2, 3);
            var l2 = new Line2D(1, 1, 2, 3);
            var l3 = new Line2D(2, 3, 1, 1);
            var l4 = new Line2D(2, 3, 5, 5);
            Assert.IsTrue(l1.Equals((object)l2));
            Assert.IsTrue(l1.Equals((object)l3));
            Assert.IsTrue(l1.Equals((object)l1));
            Assert.IsFalse(l1.Equals((object)l4));
            Assert.IsFalse(l1.Equals((object)null));
            Assert.IsFalse(l1.Equals(555));
        }
    }
}