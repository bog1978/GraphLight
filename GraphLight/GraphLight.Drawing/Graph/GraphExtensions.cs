using System;
using System.IO;
using GraphLight.Parser;

namespace GraphLight.Graph
{
    public static class GraphExtensions
    {
        #region Файловые операции

        /// <summary>
        ///   Читает граф из файла.
        /// </summary>
        /// <param name = "fileStream">Поток, из которого читать данные</param>
        /// <returns>Считанный граф</returns>
        public static DrawingGraph ReadFromFile(Stream fileStream)
        {
            var ws = new StringWriter();
            var scanner = new MyScanner(fileStream);
            var parser = new MyParser(scanner) { errors = { errorStream = ws } };
            parser.Parse();
            if (parser.errors.count > 0)
                throw new Exception(ws.ToString());
            return parser.ParsedGraph;
        }

        public static void WriteToFile(this DrawingGraph graph, Stream fileStream)
        {
            using (var sw = new StreamWriter(fileStream))
                graph.WriteToFile(sw);
        }

        public static void WriteToFile(this DrawingGraph graph, StreamWriter sw)
        {
            sw.WriteLine("digraph {0}", "Label");
            sw.WriteLine("{");

            sw.WriteLine("\t nodes:");
            foreach (var vertex in graph.Verteces)
                sw.WriteLine("\t\t{0} [label=\"{1}\" rank={2} order={3}];",
                    vertex.Data, vertex.Label, vertex.Rank, vertex.Position);

            sw.WriteLine("\t edges:");
            foreach (var myEdge in graph.Edges)
                sw.WriteLine("\t\t{0} -> {1} [color={2} thickness={3}]",
                    myEdge.Src.Data, myEdge.Dst.Data, myEdge.Color, myEdge.Thickness);

            sw.WriteLine("}");
        }

        #endregion
    }
}