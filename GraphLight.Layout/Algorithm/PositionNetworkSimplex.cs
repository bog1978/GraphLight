using System.Collections.Generic;
using System.Linq;
using GraphLight.Model;

namespace GraphLight.Algorithm
{
    internal class PositionNetworkSimplex<G, V, E> : NetworkSimplex
        where V : IVertexDataLayered, IVertexDataLocation
        where E : IEdgeDataWeight
    {
        private const int H_SPACE = 30;
        private readonly IGraph<G, V, E> _graph;
        private Dictionary<IVertex<V, E>, Vertex> _vertexMap;

        public PositionNetworkSimplex(IGraph<G, V, E> graph)
        {
            _graph = graph;
        }

        protected override void Finalze()
        {
            var minValue = int.MaxValue;
            foreach (var vertex in _graph.Vertices)
            {
                var v = _vertexMap[vertex];
                if (v.Value - vertex.Data.Rect.Width / 2 < minValue)
                    minValue = v.Value - (int)(vertex.Data.Rect.Width / 2);
            }

            foreach (var vertex in _graph.Vertices)
            {
                var v = _vertexMap[vertex];
                vertex.Data.Rect.Left = v.Value - minValue - vertex.Data.Rect.Width / 2;
            }
        }

        protected override void Initialize(out ICollection<Vertex> vertices, out ICollection<Edge> edges)
        {
            _vertexMap = _graph.Vertices.ToDictionary(x => x, vertex => new Vertex());

            vertices = _vertexMap.Values.ToList();
            edges = new List<Edge>();

            foreach (var edge in _graph.Edges)
            {
                var ve = new Vertex();
                vertices.Add(ve);
                edges.Add(new Edge(ve, _vertexMap[edge.Src], (int)edge.Data.Weight, 0));
                edges.Add(new Edge(ve, _vertexMap[edge.Dst], (int)edge.Data.Weight, 0));
            }

            foreach (var rank in _graph.GetRankList())
            {
                var spaceEdges = rank.Zip(rank.Skip(1), (v, w) =>
                    {
                        var sv = _vertexMap[v];
                        var sw = _vertexMap[w];
                        return new Edge(sv, sw, 0, (int)(v.Data.Rect.Width + w.Data.Rect.Width) / 2 + H_SPACE);
                    });
                foreach (var edge in spaceEdges)
                    edges.Add(edge);
            }
        }
    }
}