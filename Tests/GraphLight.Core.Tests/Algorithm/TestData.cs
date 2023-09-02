using GraphLight.Model;

namespace GraphLight.Algorithm
{
    internal static class TestData
    {
        /// <summary>
        /// Пример графа из Wikipedia <a href="https://ru.wikipedia.org/wiki/Минимальное_остовное_дерево"/>
        /// Минимальное остовное дерево = 38: ac, be, ce, df, ef, fg, gh, hi, ij
        /// </summary>
        public static IGraph<object, string, EdgeDataWeight> WikipediaMinSpanningTree() => Graph
            .CreateInstance<object, string, EdgeDataWeight>("")
            .AddEdgeRange(
                ("a", "b", 6),
                ("a", "c", 3),
                ("a", "d", 9),
                ("b", "c", 4),
                ("b", "e", 2),
                ("b", "g", 9),
                ("c", "d", 9),
                ("c", "e", 2),
                ("c", "f", 9),
                ("d", "f", 8),
                ("d", "j", 18),
                ("e", "g", 9),
                ("e", "f", 8),
                ("f", "g", 7),
                ("f", "i", 9),
                ("f", "j", 10),
                ("g", "h", 4),
                ("g", "i", 5),
                ("h", "i", 1),
                ("h", "j", 4),
                ("i", "j", 3));

        /// <summary>
        /// Не помню, где взял этот пример. Хотелось бы найти источник.
        /// Минимальное остовное дерево = 37: AB, BC, HG, IC, GF, CF, CD, DE
        /// ВНИМАНИЕ!!! Если вес(A,H)=8, то Prim и Kruskal неходят разные деревья с одинаковым весом.
        /// </summary>
        public static IGraph<object, string, EdgeDataWeight> SampleMinSpanningTree() => Graph
            .CreateInstance<object, string, EdgeDataWeight>("")
            .AddEdgeRange(
                ("A", "B", 4),
                ("B", "C", 8),
                ("A", "H", 9),
                ("B", "H", 11),
                ("H", "I", 7),
                ("H", "G", 1),
                ("I", "G", 6),
                ("I", "C", 2),
                ("G", "F", 2),
                ("C", "F", 4),
                ("C", "D", 7),
                ("D", "F", 14),
                ("D", "E", 9),
                ("F", "E", 10));
    }
}
