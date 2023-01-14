using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphLight.Geometry
{
    /// <summary>
    ///This is a test class for Point2DTest and is intended
    ///to contain all Point2DTest Unit Tests
    ///</summary>
    [TestClass]
    public class Point2DTest
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
        ///A test for op_Subtraction
        ///</summary>
        [TestMethod]
        public void OpSubtractionTest1()
        {
            var p1 = new Point2D(1, 2);
            var p2 = new Point2D(1, -3);
            var expected = new Vector2D(0, -5);
            var actual = (p2 - p1);
            Assert.IsTrue(expected == actual);
        }

        /// <summary>
        ///A test for op_Subtraction
        ///</summary>
        [TestMethod]
        public void OpSubtractionTest()
        {
            var p = new Point2D(1, 2);
            var v = new Vector2D(1, -3);
            var expected = new Point2D(0, 5);
            var actual = (p - v);
            Assert.IsTrue(expected == actual);
        }

        /// <summary>
        ///A test for op_Addition
        ///</summary>
        [TestMethod]
        public void OpAdditionTest()
        {
            var p = new Point2D(1, 2);
            var v = new Vector2D(1, -3);
            var expected = new Point2D(2, -1);
            var actual = (p + v);
            Assert.IsTrue(expected == actual);
        }

        /// <summary>
        ///A test for Point2D Constructor
        ///</summary>
        [TestMethod]
        public void Point2DConstructorTest()
        {
            var p = new Point2D(1, 2);
            Assert.IsTrue(p.X == 1 && p.Y == 2);
        }

        /// <summary>
        ///A test for op_Inequality
        ///</summary>
        [TestMethod]
        public void OpInequalityTest()
        {
            var p1 = new Point2D(1, 2);
            var p2 = new Point2D(3, 4);
            Assert.IsTrue(p1 != p2);
        }

        /// <summary>
        ///A test for op_Equality
        ///</summary>
        [TestMethod]
        public void OpEqualityTest()
        {
            var p1 = new Point2D(1, 2);
            var p2 = new Point2D(1, 2);
            Assert.IsTrue(p1 == p2);
        }

        /// <summary>
        ///A test for Point2D Constructor
        ///</summary>
        [TestMethod]
        public void Point2DConstructorTest1()
        {
            var target = new Point2D();
            Assert.IsTrue(target.X == 0 && target.Y == 0);
        }

        [TestMethod]
        public void CompareToNull()
        {
            Point2D l1 = null;
            var r1 = l1 == null;
            Assert.IsTrue(r1);

            var l2 = new Point2D();
            var r2 = l2 == null;
            Assert.IsFalse(r2);
        }

        [TestMethod]
        public void HashCodeTest()
        {
            var dict = new Dictionary<Point2D, string>();
            var p1 = new Point2D(1, 2);
            var p2 = new Point2D(2, 3);
            var p3 = new Point2D(1, 2);
            dict.Add(p1, "p1");
            dict.Add(p2, "p2");
            Assert.IsTrue(dict.ContainsKey(p1));
            Assert.IsTrue(dict.ContainsKey(p3));
        }

        [TestMethod]
        public void IsEmptyTest()
        {
            var p1 = new Point2D(1, double.NaN);
            var p2 = new Point2D(double.NaN, 1);
            var p3 = new Point2D(double.NaN, double.NaN);
            Assert.IsTrue(p1.IsEmpty);
            Assert.IsTrue(p2.IsEmpty);
            Assert.IsTrue(p3.IsEmpty);
        }

        [TestMethod]
        public void ObjectEqualsTest()
        {
            var p1 = new Point2D(1, 2);
            var p2 = new Point2D(1, 2);
            Assert.IsTrue(p1.Equals((object)p1));
            Assert.IsTrue(p1.Equals((object)p2));
            Assert.IsFalse(p1.Equals((object)null));
            Assert.IsFalse(p1.Equals(111));
        }
    }
}