using System.Collections.Generic;
using System.Linq;
using GraphLight.Algorithm;
using GraphLight.Graph;

namespace GraphLight.Layout
{
    public class RankNetworkSimplex : NetworkSimplex
    {
        private readonly IGraph _graph;
        private Dictionary<IVertex<IVertexData, IEdgeData>, Vertex> _vertexMap;

        public RankNetworkSimplex(IGraph graph)
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
            _vertexMap = _graph.Vertices.ToDictionary(x => (IVertex<IVertexData, IEdgeData>)x, x => new Vertex());

            vertices = _vertexMap.Values.ToList();
            edges = _graph.Edges.Where(edge => edge.Src != edge.Dst)
                .Select(x => new Edge(_vertexMap[x.Src], _vertexMap[x.Dst], (int)x.Weight, 1))
                .ToList();
        }
    }
}