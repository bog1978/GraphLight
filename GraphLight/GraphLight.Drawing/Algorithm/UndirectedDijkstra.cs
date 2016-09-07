using System;
using System.Collections.Generic;
using System.Linq;
using GraphLight.Collections;
using GraphLight.Graph;

namespace GraphLight.Algorithm
{
    public class UndirectedDijkstra<TVertex, TEdge> : IShortestPath<TVertex, TEdge> where TEdge : new()
    {
        private readonly Graph<TVertex, TEdge> _graph;
        private Action<Edge<TVertex, TEdge>> _enterEdge = x => { };
        private Action<Vertex<TVertex, TEdge>> _enterNode = x => { };

        public UndirectedDijkstra(Graph<TVertex, TEdge> graph)
        {
            _graph = graph;
        }

        #region IShortestPath<TNode,TEdge> Members

        public void Find(Vertex<TVertex, TEdge> start, Vertex<TVertex, TEdge> end)
        {
            Find(start.Data, end.Data);
        }

        public void Find(TVertex start, TVertex end)
        {
            var from = _graph[start];
            var to = _graph[end];
            var attrs = _graph.Verteces.ToDictionary(x => x, x => new DijkstraAttr());
            attrs[from].Distance = 0;

            var queue = new PriorityQueue<double, Vertex<TVertex,TEdge>>(
                _graph.Verteces, HeapType.Min);

            while (!queue.IsEmpty)
            {
                var src = queue.Dequeue();
                foreach (var edge in src.Edges)
                {
                    var dst = edge.Dst != src ? edge.Dst : edge.Src;
                    var dstAttr = attrs[dst];
					var srcAttr = attrs[src];
                    if (srcAttr.Distance + edge.Weight < dstAttr.Distance)
                    {
                        dstAttr.Parent = edge;
                        dstAttr.Distance = srcAttr.Distance + edge.Weight;
                        queue.Remove(dst);
                        queue.Add(dst);
                    }
                }
            }

            var vertexPath = new List<Vertex<TVertex, TEdge>>();
            var edgePath = new List<Edge<TVertex, TEdge>>();

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

        public Action<Edge<TVertex, TEdge>> EnterEdge
        {
            get { return _enterEdge; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _enterEdge = value;
            }
        }

        public Action<Vertex<TVertex, TEdge>> EnterNode
        {
            get { return _enterNode; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _enterNode = value;
            }
        }

        #endregion

        private class DijkstraAttr
        {
            public DijkstraAttr()
            {
                Parent = null;
                Distance = double.MaxValue;
            }

            public Edge<TVertex, TEdge> Parent;
            public double Distance;
        }
    }
}