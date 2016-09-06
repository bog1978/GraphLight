using System.Collections.Generic;
using System.Windows.Media;
using GraphLight.Graph;
using System.Globalization;
using System;

namespace GraphLight.Parser
{
    partial class MyParser
    {
        public DrawingGraph ParsedGraph;
        private IVertex<VertexAttrs, EdgeAttrs> _node;
        private IEdge<VertexAttrs, EdgeAttrs> _edge;
        private ICollection<IEdge<VertexAttrs, EdgeAttrs>> _edgeChain;

        private void createGraph(string name)
        {
            ParsedGraph = new DrawingGraph(name);
        }

        private void createNode()
        {
            _node = ParsedGraph.AddVertex(new VertexAttrs(t.val));
        }

        private void setNodeLabel()
        {
            _node.Data.Label = t.val.Trim('\"');
        }

        private void setNodeRank()
        {
            _node.Data.Rank = Convert.ToInt32(t.val);
        }

        private void setNodePosition()
        {
            _node.Data.Position = Convert.ToInt32(t.val);
        }

        private void setNodeCategory()
        {
            _node.Data.Category = t.val.Trim('\"');
        }

        string _from;
        private void createEdgeChain()
        {
            _from = t.val;
            _edgeChain = new List<IEdge<VertexAttrs, EdgeAttrs>>();
        }

        private void createEdge()
        {
            _edge = ParsedGraph.AddEdge(new VertexAttrs(_from), new VertexAttrs(t.val), new EdgeAttrs());
            _edgeChain.Add(_edge);
            _from = t.val;
        }

        private void setColor(Color color)
        {
            foreach (var edge in _edgeChain)
                edge.Data.Color = color.ToString();
        }

        private void setThickness()
        {
            var format = new NumberFormatInfo { NumberDecimalSeparator = "." };
            var thickness = double.Parse(t.val, format);
            foreach (var edge in _edgeChain)
                edge.Data.Thickness = thickness;
        }

        private void setWeight()
        {
            var format = new NumberFormatInfo { NumberDecimalSeparator = "." };
            var weight = double.Parse(t.val, format);
            foreach (var edge in _edgeChain)
                edge.Weight = weight;
        }

        private static Color stringToColor(string argb)
        {
            var a = byte.Parse(argb.Substring(1, 2), NumberStyles.HexNumber);
            var r = byte.Parse(argb.Substring(3, 2), NumberStyles.HexNumber);
            var g = byte.Parse(argb.Substring(5, 2), NumberStyles.HexNumber);
            var b = byte.Parse(argb.Substring(7, 2), NumberStyles.HexNumber);
            return Color.FromArgb(a, r, g, b);
        }
    }
}
