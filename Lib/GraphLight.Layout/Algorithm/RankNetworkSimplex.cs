using System;
using System.Collections.Generic;
using System.Linq;
using GraphLight.Model;

namespace GraphLight.Algorithm
{
    internal class RankNetworkSimplex<G, V, E>(IGraph<G, V, E> graph) : NetworkSimplex
        where V : IVertexDataLayered, IEquatable<V>
        where E : IEdgeDataWeight
        where G : notnull
    {
        private Dictionary<V, Vertex> _vertexMap = new();

        protected override void Finalze()
        {
            foreach (var vertex in graph.Vertices)
            {
                var v = _vertexMap[vertex];
                vertex.Rank = v.Value;
            }
        }

        protected override void Initialize(out ICollection<Vertex> vertices, out ICollection<Edge> edges)
        {
            _vertexMap = graph.Vertices.ToDictionary(x => x, x => new Vertex(x));

            vertices = _vertexMap.Values.ToList();
            edges = graph.Edges.Where(edge => !edge.Src.Equals(edge.Dst))
                .Select(x => new Edge(_vertexMap[x.Src], _vertexMap[x.Dst], (int)x.Data.Weight, 1))
                .ToList();
        }
    }
}