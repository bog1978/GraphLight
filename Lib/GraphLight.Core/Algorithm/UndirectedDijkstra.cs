using System;
using System.Collections.Generic;
using System.Linq;
using GraphLight.Collections;
using GraphLight.Model;

namespace GraphLight.Algorithm
{
    internal class UndirectedDijkstra<G, V, E> : IShortestPath<V, E>
        where E : IEdgeDataWeight
        where V : IEquatable<V>
        where G : notnull
    {
        private readonly IGraph<G, V, E> _graph;
        private Action<IEdge<V, E>> _enterEdge = _ => { };
        private Action<V> _enterNode = _ => { };

        public UndirectedDijkstra(IGraph<G, V, E> graph)
        {
            _graph = graph;
        }

        #region IShortestPath<TNode,TEdge> Members

        public void Execute(V start, V end)
        {
            var from = start;
            var to = end;
            var attrs = _graph.Vertices.ToDictionary(x => x, _ => new DijkstraAttr());
            attrs[from].Distance = 0;

            var queue = new Collections.PriorityQueue<double, V>(_graph.Vertices, x => attrs[x].Distance, HeapType.Min);

            while (queue.Count > 0)
            {
                var src = queue.Dequeue();
                foreach (var edge in _graph.GetEdges(src))
                {
                    var dst = !src.Equals(edge.Dst) ? edge.Dst : edge.Src;
                    var dstAttr = attrs[dst];
                    var srcAttr = attrs[src];
                    if (srcAttr.Distance + edge.Data.Weight < dstAttr.Distance)
                    {
                        dstAttr.Parent = edge;
                        dstAttr.Distance = srcAttr.Distance + edge.Data.Weight;
                        queue.Remove(dst);
                        queue.Enqueue(dst, dstAttr.Distance);
                    }
                }
            }

            var vertexPath = new List<V>();
            var edgePath = new List<IEdge<V, E>>();

            var last = to;
            while (!last.Equals(from))
            {
                vertexPath.Add(last);
                var edge = attrs[last].Parent;
                if (edge != null)
                {
                    edgePath.Add(edge);
                    last = !edge.Src.Equals(last) ? edge.Src : edge.Dst;
                }
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

        public Action<V> EnterNode
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