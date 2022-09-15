using System;
using System.Diagnostics;
using GraphLight.Drawing;
using GraphLight.Graph;
using GraphLight.Layout;

namespace GraphLight.Prefomance
{
    public class LayoutPerfomanceTest : IPerfomanceTest
    {
        public LayoutPerfomanceTest()
        {
            IterCount = 10;
        }

        private static void doTest()
        {
            var graph = TestData.GraphToTest;
            var layout = new GraphVizLayout<IVertexData, IEdgeData>
            {
                NodeMeasure = new WpfNodeMeasure<IVertexData, IEdgeData>(),
                Graph = graph
            };
            layout.Layout();
        }

        public void Warmup()
        {
            doTest();
        }

        public void Test()
        {
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < IterCount; i++)
            {
                Console.Write("Iteration: {0}", i);
                doTest();
                Console.Write("\r");
            }
            Console.WriteLine();
            sw.Stop();
            Console.WriteLine("Elapsed milliseconds: {0}", sw.ElapsedMilliseconds);
        }

        public int IterCount { get; set; }
    }
}
