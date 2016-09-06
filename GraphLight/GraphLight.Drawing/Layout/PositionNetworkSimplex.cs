using System.Collections.Generic;
using System.Linq;
using GraphLight.Algorithm;
using GraphLight.Graph;

namespace GraphLight.Layout
{
    internal class PositionNetworkSimplex<TVertex, TEdge> : NetworkSimplex
        where TVertex : IVertexAttrs, new()
        where TEdge : IEdgeAttrs, new()
    {
        private const int H_SPACE = 30;
        private readonly IGraph<TVertex, TEdge> _graph;
        private int _num = 1;
        private Dictionary<IVertex<TVertex, TEdge>, Vertex> _vertexMap;

        public PositionNetworkSimplex(IGraph<TVertex, TEdge> graph)
        {
            _graph = graph;
        }

        protected override void Finalze()
        {
            var minValue = int.MaxValue;
            foreach (var vertex in _graph.Verteces)
            {
                var v = _vertexMap[vertex];
                if (v.Value - vertex.Data.Width / 2 < minValue)
                    minValue = v.Value - (int)(vertex.Data.Width / 2);
            }

            foreach (var vertex in _graph.Verteces)
            {
                var v = _vertexMap[vertex];
                vertex.Data.CenterX = v.Value - minValue;
                vertex.Data.Left = v.Value - minValue - vertex.Data.Width / 2;
            }
        }

        protected override void Initialize(out ICollection<Vertex> vertices, out ICollection<Edge> edges)
        {
            _vertexMap = _graph.Verteces.ToDictionary(x => x, vertex => new Vertex(vertex.Data.Id));

            vertices = _vertexMap.Values.ToList();
            edges = new List<Edge>();

            foreach (var edge in _graph.Edges)
            {
                var ve = new Vertex("tmp" + _num++);
                vertices.Add(ve);
                edges.Add(new Edge(ve,_vertexMap[edge.Src],(int)edge.Weight,0));
                edges.Add(new Edge(ve,_vertexMap[edge.Dst],(int)edge.Weight,0));
            }

            foreach (var rank in _graph.GetRankList())
            {
                var spaceEdges = rank.Zip(rank.Skip(1), (v, w) =>
                    {
                        var sv = _vertexMap[v];
                        var sw = _vertexMap[w];
                        return new Edge(sv,sw,0,((int)(v.Data.Width + w.Data.Width)) / 2 + H_SPACE);
                    });
                foreach (var edge in spaceEdges)
                    edges.Add(edge);
            }
        }
    }
}