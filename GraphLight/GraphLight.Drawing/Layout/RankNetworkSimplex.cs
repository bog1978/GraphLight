using System.Collections.Generic;
using System.Linq;
using GraphLight.Algorithm;
using GraphLight.Graph;

namespace GraphLight.Layout
{
    internal class RankNetworkSimplex<TVertex, TEdge> : NetworkSimplex
        where TVertex : IVertexAttrs, new()
        where TEdge : IEdgeAttrs, new()
    {
        private readonly IGraph<TVertex, TEdge> _graph;
        private Dictionary<IVertex<TVertex, TEdge>, Vertex> _vertexMap;

        public RankNetworkSimplex(IGraph<TVertex, TEdge> graph)
        {
            _graph = graph;
        }

        protected override void Finalze()
        {
            foreach (var vertex in _graph.Verteces)
            {
                var v = _vertexMap[vertex];
                vertex.Data.Rank = v.Value;
            }
        }

        protected override void Initialize(out ICollection<Vertex> vertices, out ICollection<Edge> edges)
        {
            _vertexMap = _graph.Verteces.ToDictionary(x => x, x => new Vertex(x.Data.Id));

            vertices = _vertexMap.Values.ToList();
            edges = _graph.Edges.Where(edge => edge.Src != edge.Dst)
                .Select(x => new Edge(_vertexMap[x.Src], _vertexMap[x.Dst], (int)x.Weight, 1))
                .ToList();
        }
    }
}