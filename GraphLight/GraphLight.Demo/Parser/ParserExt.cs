using System.Collections.Generic;
using System.Windows.Media;
using GraphLight.Graph;
using System.Globalization;
using System;

namespace GraphLight.Parser
{
    partial class MyParser
    {
        public IGraph ParsedGraph;
        private IVertex _node;
        private IEdge _edge;
        private ICollection<IEdge> _edgeChain;

        private void createGraph(string name)
        {
            ParsedGraph = new GraphModel();
        }

        private void createNode()
        {
            _node = ParsedGraph.AddVertex(t.val);
            _node.Label = t.val;
        }

        private void setNodeLabel()
        {
            _node.Label = t.val.Trim('\"');
        }

        private void setNodeRank()
        {
            _node.Rank = Convert.ToInt32(t.val);
        }

        private void setNodePosition()
        {
            _node.Position = Convert.ToInt32(t.val);
        }

        private void setNodeCategory()
        {
            _node.Category = t.val.Trim('\"');
        }

        string _from;
        private void createEdgeChain()
        {
            _from = t.val;
            _edgeChain = new List<IEdge>();
        }

        private void createEdge()
        {
            _edge = ParsedGraph.AddEdge(_from, t.val);
            if(_edge.Src.Label == null)
                _edge.Src.Label = _from;
            if (_edge.Dst.Label == null)
                _edge.Dst.Label = t.val;
            _edgeChain.Add(_edge);
            _from = t.val;
        }

        private void setColor(Color color)
        {
            foreach (var edge in _edgeChain)
                edge.Color = color.ToString();
        }

        private void setThickness()
        {
            var format = new NumberFormatInfo { NumberDecimalSeparator = "." };
            var thickness = double.Parse(t.val, format);
            foreach (var edge in _edgeChain)
                edge.Thickness = thickness;
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
