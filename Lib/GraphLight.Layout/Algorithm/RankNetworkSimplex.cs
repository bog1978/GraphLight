using System;
using System.Collections.Generic;
using System.Linq;
using GraphLight.Model;

namespace GraphLight.Algorithm
{
    internal class RankNetworkSimplex<G, V, E> : NetworkSimplex
        where V : IVertexDataLayered, IEquatable<V>
        where E : IEdgeDataWeight
    {
        private readonly IGraph<G, V, E> _graph;
        private Dictionary<V, Vertex> _vertexMap;

        public RankNetworkSimplex(IGraph<G, V, E> graph)
        {
            _graph = graph;
        }

        protected override void Finalze()
        {
            foreach (var vertex in _graph.Vertices)
            {
                var v = _vertexMap[vertex];
                vertex.Rank = v.Value;
            }
        }

        protected override void Initialize(out ICollection<Vertex> vertices, out ICollection<Edge> edges)
        {
            _vertexMap = _graph.Vertices.ToDictionary(x => x, x => new Vertex(x));

            vertices = _vertexMap.Values.ToList();
            edges = _graph.Edges.Where(edge => !edge.Src.Equals(edge.Dst))
                .Select(x => new Edge(_vertexMap[x.Src], _vertexMap[x.Dst], (int)x.Data.Weight, 1))
                .ToList();
        }
    }
}