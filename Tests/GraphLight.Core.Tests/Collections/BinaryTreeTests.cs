using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GraphLight.Collections.Tests
{
    [TestClass]
    public class BinaryTreeTests
    {
        [TestMethod]
        public void AddTest()
        {
            Console.WriteLine("Тест 1");
            var s = "JFPDGLVCNSXQU";
            BinaryTreeNode<char> root = null;

            foreach (var ch in s)
                root = root.Insert(ch);
            root.Dump("");
            Assert.AreEqual("[J:[F:[D:C-]-G]-[P:[L:-N]-[V:[S:Q-U]-X]]]", root.ToString());

            root = root.Remove('J');
            root.Dump("");
            Assert.AreEqual("[L:[F:[D:C-]-G]-[S:[P:N-Q]-[V:U-X]]]", root.ToString());

            root = root.Remove('X');
            root.Dump("");
            Assert.AreEqual("[L:[F:[D:C-]-G]-[S:[P:N-Q]-[V:U-]]]", root.ToString());
        }
    }
}