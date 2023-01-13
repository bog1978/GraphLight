using System;
using System.Collections.Generic;
using System.Linq;
using GraphLight.Collections;
using GraphLight.Graph;

namespace GraphLight.Algorithm
{
    internal class UndirectedDijkstra<G, V, E> : IShortestPath<V, E>
        where E : IEdgeDataWeight
    {
        private readonly IGraph<G, V, E> _graph;
        private Action<IEdge<V, E>> _enterEdge = x => { };
        private Action<IVertex<V, E>> _enterNode = x => { };

        public UndirectedDijkstra(IGraph<G, V, E> graph)
        {
            _graph = graph;
        }

        #region IShortestPath<TNode,TEdge> Members

        public void Execute(V start, V end)
        {
            var from = _graph[start];
            var to = _graph[end];
            var attrs = _graph.Vertices.ToDictionary(x => x, x => new DijkstraAttr());
            attrs[from].Distance = 0;

            var queue = new PriorityQueue<double, IVertex<V, E>>(_graph.Vertices, HeapType.Min);

            while (!queue.IsEmpty)
            {
                var src = queue.Dequeue();
                foreach (var edge in src.Edges)
                {
                    var dst = edge.Dst != src ? edge.Dst : edge.Src;
                    var dstAttr = attrs[dst];
                    var srcAttr = attrs[src];
                    if (srcAttr.Distance + edge.Data.Weight < dstAttr.Distance)
                    {
                        dstAttr.Parent = edge;
                        dstAttr.Distance = srcAttr.Distance + edge.Data.Weight;
                        queue.Remove(dst);
                        queue.Enqueue(dst);
                    }
                }
            }

            var vertexPath = new List<IVertex<V, E>>();
            var edgePath = new List<IEdge<V, E>>();

            var last = to;
            while (last != from)
            {
                var edge = attrs[last].Parent;
                edgePath.Add(edge);
                vertexPath.Add(last);
                last = edge.Src != last ? edge.Src : edge.Dst;
            }
            vertexPath.Add(from);

            vertexPath.Reverse();
            edgePath.Reverse();
            foreach (var vertex in vertexPath)
                EnterNode(vertex);
            foreach (var edge in edgePath)
                EnterEdge(edge);
        }

        public Action<IEdge<V, E>> EnterEdge
        {
            get => _enterEdge;
            set => _enterEdge = value ?? throw new ArgumentNullException(nameof(value));
        }

        public Action<IVertex<V, E>> EnterNode
        {
            get => _enterNode;
            set => _enterNode = value ?? throw new ArgumentNullException(nameof(value));
        }

        #endregion

        private class DijkstraAttr
        {
            public DijkstraAttr()
            {
                Parent = null;
                Distance = double.MaxValue;
            }

            public IEdge<V, E>? Parent;
            public double Distance;
        }
    }
}