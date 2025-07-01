using System;
using System.Collections.Generic;
using System.Linq;
using GraphLight.Model;

namespace GraphLight.Algorithm
{
    internal class PositionNetworkSimplex<G, V, E>(IGraph<G, V, E> graph) : NetworkSimplex
        where V : IVertexDataLayered, IVertexDataLocation, IEquatable<V>
        where E : IEdgeDataWeight
        where G : notnull
    {
        private const int HSpace = 30;
        private Dictionary<V, Vertex> _vertexMap = new();

        protected override void Finalze()
        {
            var minValue = int.MaxValue;
            foreach (var vertex in graph.Vertices)
            {
                var v = _vertexMap[vertex];
                if (v.Value - vertex.Rect.Width / 2 < minValue)
                    minValue = v.Value - (int)(vertex.Rect.Width / 2);
            }

            foreach (var vertex in graph.Vertices)
            {
                var v = _vertexMap[vertex];
                vertex.Rect.Left = v.Value - minValue - vertex.Rect.Width / 2;
            }
        }

        protected override void Initialize(out ICollection<Vertex> vertices, out ICollection<Edge> edges)
        {
            _vertexMap = graph.Vertices.ToDictionary(x => x, vertex => new Vertex(vertex));

            vertices = _vertexMap.Values.ToList();
            edges = new List<Edge>();

            foreach (var edge in graph.Edges)
            {
                var ve = new Vertex(edge);
                vertices.Add(ve);
                edges.Add(new Edge(ve, _vertexMap[edge.Src], (int)edge.Data.Weight, 0));
                edges.Add(new Edge(ve, _vertexMap[edge.Dst], (int)edge.Data.Weight, 0));
            }

            foreach (var rank in graph.GetRankList())
            {
                var spaceEdges = rank.Zip(rank.Skip(1), (v, w) =>
                    {
                        var sv = _vertexMap[v];
                        var sw = _vertexMap[w];
                        return new Edge(sv, sw, 0, (int)(v.Rect.Width + w.Rect.Width) / 2 + HSpace);
                    });
                foreach (var edge in spaceEdges)
                    edges.Add(edge);
            }
        }
    }
}