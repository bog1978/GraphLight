using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GraphLight.Prefomance
{
    public class CollectionsPerfomanceTest : IPerfomanceTest
    {
        const int ARRAY_SIZE = 100000000;

        public void Warmup()
        {
        }

        public void Test()
        {
            arrayTest();
            GC.Collect();
            GC.Collect();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            listTest();
        }

        public int IterCount { get; set; }

        static void arrayTest()
        {
            Console.WriteLine("Testing array...");

            var arr = new int[ARRAY_SIZE];
            for (var i = 0; i < ARRAY_SIZE; i++)
                arr[i] = i;

            var sw = Stopwatch.StartNew();
            var sum = 0;
            for (var i = 0; i < ARRAY_SIZE; i++)
                sum += arr[i];
            Console.WriteLine("indexer: {0}", sw.ElapsedMilliseconds);

            sw.Restart();
            sum = 0;
            foreach (var x in arr)
                sum += x;
            Console.WriteLine("enumerator: {0}", sw.ElapsedMilliseconds);
        }

        static void listTest()
        {
            Console.WriteLine();
            Console.WriteLine("Testing list...");
            var arr = new List<int>();
            for (var i = 0; i < ARRAY_SIZE; i++)
                arr.Add(i);

            var sw = Stopwatch.StartNew();
            var sum = 0;
            for (var i = 0; i < ARRAY_SIZE; i++)
                sum += arr[i];
            Console.WriteLine("indexer: {0}", sw.ElapsedMilliseconds);

            sw.Restart();
            sum = 0;
            foreach (var x in arr)
                sum += x;
            Console.WriteLine("enumerator: {0}", sw.ElapsedMilliseconds);
        }
    }
}