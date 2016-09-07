using System;
using System.IO;
using GraphLight.Parser;

namespace GraphLight.Graph
{
    public class DrawingGraph : Graph<VertexAttrs, EdgeAttrs>
    {
        #region �������� ��������

        /// <summary>
        ///   ������ ���� �� �����.
        /// </summary>
        /// <param name = "fileStream">�����, �� �������� ������ ������</param>
        /// <returns>��������� ����</returns>
        public static DrawingGraph ReadFromFile(Stream fileStream)
        {
            var ws = new StringWriter();
            var scanner = new MyScanner(fileStream);
            var parser = new MyParser(scanner) {errors = {errorStream = ws}};
            parser.Parse();
            if (parser.errors.count > 0)
                throw new Exception(ws.ToString());
            return parser.ParsedGraph;
        }

        public void WriteToFile(Stream fileStream)
        {
            using (var sw = new StreamWriter(fileStream))
                WriteToFile(sw);
        }

        public void WriteToFile(StreamWriter sw)
        {
            sw.WriteLine("digraph {0}", "Label");
            sw.WriteLine("{");

            sw.WriteLine("\t nodes:");
            foreach (var vertex in Verteces)
                sw.WriteLine("\t\t{0} [label=\"{1}\" rank={2} order={3}];",
                             vertex.Data, vertex.Label, vertex.Rank, vertex.Position);

            sw.WriteLine("\t edges:");
            foreach (DrawingEdge myEdge in Edges)
                sw.WriteLine("\t\t{0} -> {1} [color={2} thickness={3}]",
                             myEdge.Src.Data, myEdge.Dst.Data, myEdge.Color, myEdge.Thickness);

            sw.WriteLine("}");
        }

        #endregion

        protected override IEdge<VertexAttrs, EdgeAttrs> CreateEdge(EdgeAttrs data)
        {
            return new DrawingEdge(data);
        }

        protected override IVertex<VertexAttrs, EdgeAttrs> CreateVertex(VertexAttrs data)
        {
            return new DrawingVertex(data);
        }
    }
}