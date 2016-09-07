﻿using System.Collections.Generic;
using System.Linq;
using GraphLight.Algorithm;
using GraphLight.Graph;

namespace GraphLight.Layout
{
    internal class RankNetworkSimplex<TVertex, TEdge> : NetworkSimplex
        where TVertex : VertexAttrs, new() where TEdge : new()
        //where TEdge : IEdgeAttrs, new()
    {
        private readonly Graph<TVertex, TEdge> _graph;
        private Dictionary<Vertex<TVertex, TEdge>, Vertex> _vertexMap;

        public RankNetworkSimplex(Graph<TVertex, TEdge> graph)
        {
            _graph = graph;
        }

        protected override void Finalze()
        {
            foreach (var vertex in _graph.Verteces)
            {
                var v = _vertexMap[vertex];
                vertex.Rank = v.Value;
            }
        }

        protected override void Initialize(out ICollection<Vertex> vertices, out ICollection<Edge> edges)
        {
            _vertexMap = _graph.Verteces.ToDictionary(x => x, x => new Vertex());

            vertices = _vertexMap.Values.ToList();
            edges = _graph.Edges.Where(edge => edge.Src != edge.Dst)
                .Select(x => new Edge(_vertexMap[x.Src], _vertexMap[x.Dst], (int)x.Weight, 1))
                .ToList();
        }
    }
}