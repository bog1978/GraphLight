using System;
using System.Diagnostics;

namespace GraphLight.Prefomance
{
    public abstract class BaseSimplexPerfomanceTest: IPerfomanceTest
    {
        public int IterCount { get; set; }

        protected BaseSimplexPerfomanceTest()
        {
            IterCount = 10;
        }

        public void Warmup()
        {
            RankingSimplexTest();
            //PositioningSimplexTest();
        }

        public void Test()
        {
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < IterCount; i++)
            {
                Console.Write("Iteration: {0} .", i);
                RankingSimplexTest();
                Console.Write(".");
                //PositioningSimplexTest();
                Console.Write(".");
                Console.Write(" OK!\r");
            }
            Console.WriteLine();
            sw.Stop();
            Console.WriteLine("Elapsed milliseconds: {0}", sw.ElapsedMilliseconds);
        }

        protected abstract void PositioningSimplexTest();
        protected abstract void RankingSimplexTest();
    }
}