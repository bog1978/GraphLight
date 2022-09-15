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
            ParsedGraph = new LayoutGraphModel();
        }

        private void createNode()
        {
            _node = (IVertex)ParsedGraph.AddVertex(ParsedGraph.CreateVertexData(t.val));
            _node.Data.Label = t.val;
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
            _edgeChain = new List<IEdge>();
        }

        private void createEdge()
        {
            _edge = (IEdge)ParsedGraph.AddEdge(
                ParsedGraph.CreateVertexData(_from),
                ParsedGraph.CreateVertexData(t.val),
                ParsedGraph.CreateEdgeData());
            if(_edge.Src.Data.Label == null)
                _edge.Src.Data.Label = _from;
            if (_edge.Dst.Data.Label == null)
                _edge.Dst.Data.Label = t.val;
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
