using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GraphLight.Serialization.Dot
{
    public class DotGraph : DotElement
    {
        private readonly ICollection<DotEdge> _edges = new List<DotEdge>();
        private readonly ICollection<DotGraph> _subGraphs = new List<DotGraph>();
        private readonly ICollection<DotVertex> _verteces = new List<DotVertex>();
        public string Id = "test";

        public ICollection<DotVertex> Verteces
        {
            get { return _verteces; }
        }

        public ICollection<DotEdge> Edges
        {
            get { return _edges; }
        }

        public ICollection<DotGraph> SubGraphs
        {
            get { return _subGraphs; }
        }

        public void Serialize(StreamWriter sw)
        {
            serialize(sw, this);
        }

        private static void serialize(TextWriter sw, DotGraph graph, string rootName = "digraph", string indent = "")
        {
            sw.WriteLine(indent + rootName + " " + graph.Id);
            sw.WriteLine(indent + "{");
            foreach (var pair in graph.Attrs)
                sw.WriteLine(indent + "\t" + pair.Key + " = " + pair.Value + ";");

            string strAttrs;
            foreach (var v in graph.Verteces)
            {
                strAttrs = "";
                if (v.Attrs.Any())
                {
                    var attrs = v.Attrs.Select(x => string.Format("{0}={1}", x.Key, x.Value));
                    strAttrs = "[" + string.Join(" ", attrs) + "]";
                }
                sw.WriteLine("{0}\t{1}{2};", indent, v.Id, strAttrs);
            }
            foreach (var e in graph.Edges)
            {
                strAttrs = "";
                if (e.Attrs.Any())
                {
                    var attrs = e.Attrs.Select(x => string.Format("{0}={1}", x.Key, x.Value));
                    strAttrs = "[" + string.Join(" ", attrs) + "]";
                }
                sw.WriteLine("{0}\t{1}->{2}{3};", indent, e.Src, e.Dst, strAttrs);
            }
            foreach (var g in graph.SubGraphs)
            {
                serialize(sw, g, "subgraph", indent + "\t");
            }
            sw.WriteLine(indent + "}");
        }
    }
}