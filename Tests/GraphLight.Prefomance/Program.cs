using System;

namespace GraphLight.Prefomance
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Layout test.");
            executeTest(new LayoutPerfomanceTest());
            //Console.WriteLine("\nAlgebraic simplex test.");
            //executeTest(new AlgebraicSimplexPerfomanceTest());
        }

        private static void executeTest(IPerfomanceTest perfomanceTest)
        {
            perfomanceTest.IterCount = 10;

            perfomanceTest.Warmup();

            //Console.WriteLine("Attach profiler then press ENTER");
            //Console.ReadLine();

            perfomanceTest.Test();

            //Console.WriteLine("Detach profiler then press ENTER");
            //Console.ReadLine();
        }
    }
}
