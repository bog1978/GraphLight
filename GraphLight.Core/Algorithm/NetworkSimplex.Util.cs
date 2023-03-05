using System.Collections.Generic;
using System.Linq;
using GraphLight.Collections;

namespace GraphLight.Algorithm
{
    partial class NetworkSimplex
    {
        private static Graph MakeRootedGraph(ICollection<Vertex> vertices, ICollection<Edge> edges)
        {
            var root = new Vertex("_ROOT_");
            var roots = vertices.Except(edges.Select(x => x.Dst)).ToArray();
            foreach (var vertex in roots)
                edges.Add(new Edge(root, vertex, 0, 1));
            vertices.Add(root);

            foreach (var grp in edges.GroupBy(e => e.Src))
                grp.Key.OutEdges = grp.ToArray();

            foreach (var grp in edges.GroupBy(e => e.Dst))
                grp.Key.InEdges = grp.ToArray();

            foreach (var vertex in vertices)
            {
                if (vertex.InEdges == null)
                    vertex.InEdges = new Edge[] { };
                if (vertex.OutEdges == null)
                    vertex.OutEdges = new Edge[] { };
                vertex.Edges = vertex.InEdges.Union(vertex.OutEdges).ToArray();
            }

            var graph = new Graph { Vertices = vertices.ToArray(), Edges = edges.ToArray(), Root = root };
            return graph;
        }

        private static void SpanningTree(Graph graph)
        {
            graph.Root.Priority = 0;
            var heap = new BinaryHeap<int, Vertex>(graph.Vertices, x => x.Priority, HeapType.Min);
            while (heap.Count > 0)
            {
                var u = heap.RemoveRoot();
                u.Color = VertexColor.Black;

                if (u.ParentEdge != null)
                    u.ParentEdge.IsTree = true;

                foreach (var e in u.Edges)
                {
                    var v = e.Src == u ? e.Dst : e.Src;
                    var weight = e.Length;
                    if (v.Color != VertexColor.White || weight >= v.Priority)
                        continue;
                    v.ParentEdge = e;
                    v.Priority = weight;
                    heap.Remove(v);
                    heap.Add(weight, v);
                }
            }
        }

        private static void PostOrderTraversal(Graph graph, Vertex root)
        {
            int min, max, lim;

            if (root.ParentEdge != null)
            {
                min = root.Low;
                max = root.Lim;
                lim = min;
                // ParentEdge is at index 0.
                root.ScanIndex = 1;
            }
            else
            {
                min = 0;
                max = graph.Vertices.Length - 1;
                lim = 0;
                root.ScanIndex = 0;
            }

            // root has index max in _graph.Vertices.
            for (var i = min; i < max; i++)
            {
                var v = graph.Vertices[i];
                v.Low = int.MaxValue;
                v.Lim = int.MaxValue;
                v.ParentEdge = null;
                v.ParentVertex = null;
                // For each vertex except global root Edges collection
                // will contain ParentEdge at index 0.
                v.ScanIndex = 1;
            }

            var curr = root;
            while (true)
            {
                var len = curr.TreeEdgeCount;
                if (curr.ScanIndex < len)
                {
                    var edge = curr.Edges[curr.ScanIndex++];
                    var next = edge.Src == curr ? edge.Dst : edge.Src;
                    next.ParentEdge = edge;
                    next.ParentVertex = curr;
                    var edgeIndex = 0;
                    for (; edgeIndex < next.TreeEdgeCount; edgeIndex++)
                        if (next.Edges[edgeIndex] == edge)
                            break;
                    if (edgeIndex > 0)
                    {
                        // Move ParentEdge to the index 0.
                        var tmp = next.Edges[0];
                        next.Edges[0] = edge;
                        next.Edges[edgeIndex] = tmp;
                    }
                    curr = next;
                }
                else
                {
                    if (curr.Low == int.MaxValue)
                        curr.Low = lim;
                    if (curr.Lim != lim)
                    {
                        curr.Lim = lim;
                        graph.Vertices[lim] = curr;
                    }
                    lim++;
                    var prev = curr.ParentVertex;
                    if (prev == root.ParentVertex)
                        break;
                    if (prev.Low > curr.Low)
                        prev.Low = curr.Low;
                    curr = prev;
                }
            }
        }
    }
}