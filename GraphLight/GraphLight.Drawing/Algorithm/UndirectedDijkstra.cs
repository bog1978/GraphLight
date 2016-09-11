using System;
using System.Collections.Generic;
using System.Linq;
using GraphLight.Collections;
using GraphLight.Graph;

namespace GraphLight.Algorithm
{
    public class UndirectedDijkstra :  IShortestPath
    {
        private readonly IGraph _graph;
        private Action<IEdge> _enterEdge = x => { };
        private Action<IVertex> _enterNode = x => { };

        public UndirectedDijkstra(IGraph graph)
        {
            _graph = graph;
        }

        #region IShortestPath<TNode,TEdge> Members

        public void Find(object start, object end)
        {
            var from = _graph[start];
            var to = _graph[end];
            var attrs = _graph.Verteces.ToDictionary(x => x, x => new DijkstraAttr());
            attrs[from].Distance = 0;

            var queue = new PriorityQueue<double, IVertex>(_graph.Verteces, HeapType.Min);

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

            var vertexPath = new List<IVertex>();
            var edgePath = new List<IEdge>();

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

        public Action<IEdge> EnterEdge
        {
            get { return _enterEdge; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _enterEdge = value;
            }
        }

        public Action<IVertex> EnterNode
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

            public IEdge Parent;
            public double Distance;
        }
    }
}