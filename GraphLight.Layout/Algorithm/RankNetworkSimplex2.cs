using System;
using System.Collections.Generic;
using GraphLight.Model;

namespace GraphLight.Algorithm
{
    internal class RankNetworkSimplex2<G, V, E> : NetworkSimplex2
        where V : IVertexDataLayered, IEquatable<V>
        where E : IEdgeDataWeight
    {
        private readonly IGraph<G, V, E> _graph;
        private readonly Dictionary<V, VertexData> _vertexMap;

        public RankNetworkSimplex2(IGraph<G, V, E> graph)
        {
            _graph = graph;
            _vertexMap = new Dictionary<V, VertexData>();
        }

        protected override void Initialize(IGraph<GraphData, VertexData, EdgeData> graph)
        {
            foreach (var vertex in _graph.Vertices)
            {
                var vertexData = new VertexData(vertex);
                _vertexMap.Add(vertex, vertexData);
                graph.AddVertex(vertexData);
            }

            foreach (var edge in _graph.Edges)
            {
                if(edge.IsLoop())
                    continue;
                var src = _vertexMap[edge.Src];
                var dst = _vertexMap[edge.Dst];
                var edgeData = new EdgeData((int)edge.Data.Weight, 1);
                graph.AddEdge(src, dst, edgeData);
            }
        }

        protected override void Finalze()
        {
            foreach (var vertex in _graph.Vertices)
            {
                var v = _vertexMap[vertex];
                vertex.Rank = v.Value;
            }
        }
    }
}