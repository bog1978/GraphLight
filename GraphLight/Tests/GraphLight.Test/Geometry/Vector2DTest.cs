using GraphLight.Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphLight.Test.Geometry
{
    /// <summary>
    ///This is a test class for Vector2DTest and is intended
    ///to contain all Vector2DTest Unit Tests
    ///</summary>
    [TestClass]
    public class Vector2DTest
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
        ///A test for Len
        ///</summary>
        [TestMethod]
        public void LenTest()
        {
            var target = new Vector2D(3, 4);
            Assert.IsTrue(target.Len == 5);
        }

        /// <summary>
        ///A test for op_Subtraction
        ///</summary>
        [TestMethod]
        public void OpSubtractionTest()
        {
            var a = new Vector2D(1, 2);
            var b = new Vector2D(1, -3);
            var expected = new Vector2D(0, 5);
            var actual = (a - b);
            Assert.IsTrue(expected == actual);
        }

        /// <summary>
        ///A test for op_Multiply
        ///</summary>
        [TestMethod]
        public void OpMultiplyTest()
        {
            var v = new Vector2D(1, 2);
            var expected = new Vector2D(5, 10);
            var actual = v * 5;
            Assert.IsTrue(expected == actual);
        }

        /// <summary>
        /// Тест векторного произведения
        ///</summary>
        [TestMethod]
        public void OpExclusiveOrTest()
        {
            var a = new Vector2D(0, 3);
            var b = new Vector2D(4, 0);
            var actual1 = (a ^ b);
            Assert.IsTrue(actual1 == -12);

            var actual2 = (b ^ a);
            Assert.IsTrue(actual2 == 12);
        }

        /// <summary>
        ///A test for op_Division
        ///</summary>
        [TestMethod]
        public void OpDivisionTest()
        {
            var v = new Vector2D(5, 10);
            var expected = new Vector2D(1, 2);
            var actual = v / 5;
            Assert.IsTrue(expected == actual);
        }

        /// <summary>
        ///A test for op_Addition
        ///</summary>
        [TestMethod]
        public void OpAdditionTest()
        {
            var a = new Vector2D(1, 2);
            var b = new Vector2D(1, -3);
            var expected = new Vector2D(2, -1);
            var actual = (a + b);
            Assert.IsTrue(expected == actual);
        }

        /// <summary>
        ///A test for Vector2D Constructor
        ///</summary>
        [TestMethod]
        public void Vector2DConstructorTest1()
        {
            var target = new Vector2D(1, -2);
            Assert.IsTrue(target.X == 1 && target.Y == -2);
        }

        /// <summary>
        ///A test for Vector2D Constructor
        ///</summary>
        [TestMethod]
        public void Vector2DConstructorTest()
        {
            var p1 = new Point2D(1, 2);
            var p2 = new Point2D(1, -3);
            var target = new Vector2D(p1, p2);
            Assert.IsTrue(target.X == 0 && target.Y == -5);
        }

        /// <summary>
        ///A test for op_Inequality
        ///</summary>
        [TestMethod]
        public void OpInequalityTest()
        {
            var p1 = new Vector2D(1, 2);
            var p2 = new Vector2D(3, 4);
            Assert.IsTrue(p1 != p2);
        }

        /// <summary>
        ///A test for op_Equality
        ///</summary>
        [TestMethod]
        public void OpEqualityTest()
        {
            var p1 = new Vector2D(1, 2);
            var p2 = new Vector2D(1, 2);
            Assert.IsTrue(p1 == p2);
        }

        /// <summary>
        ///A test for Vector2D Constructor
        ///</summary>
        [TestMethod]
        public void Vector2DConstructorTest2()
        {
            var target = new Vector2D();
            Assert.IsTrue(target.X == 0 && target.Y == 0);
        }

        [TestMethod]
        public void CompareToNull()
        {
            Vector2D l1 = null;
            var r1 = l1 == null;
            Assert.IsTrue(r1);

            Vector2D l2 = new Vector2D();
            var r2 = l2 == null;
            Assert.IsFalse(r2);
        }
    }
}