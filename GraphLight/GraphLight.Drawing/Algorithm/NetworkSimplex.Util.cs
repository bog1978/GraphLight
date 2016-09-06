using System.Collections.Generic;
using System.Linq;
using GraphLight.Collections;
using GraphLight.Graph;

namespace GraphLight.Algorithm
{
    partial class NetworkSimplex
    {
        private static Graph makeRootedGraph(ICollection<Vertex> vertices, ICollection<Edge> edges)
        {
            var root = new Vertex("_root_");
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

            var graph = new Graph { Verteces = vertices.ToArray(), Edges = edges.ToArray(), Root = root };
            return graph;
        }

        private static void spanningTree(Graph graph)
        {
            graph.Root.HeapKey = 0;
            var heap = new BinaryHeap<int, Vertex>(graph.Verteces, HeapType.Min);
            while (heap.Count > 0)
            {
                var u = heap.RemoveRoot();
                u.Color = VertexColor.Black;

                if (u.ParentEdge != null)
                    u.ParentEdge.IsTree = true;

                foreach (var e in u.Edges)
                {
                    var v = e.Src == u ? e.Dst : e.Src;
                    var weight = e.Lenght;
                    if (v.Color != VertexColor.White || weight >= v.HeapKey)
                        continue;
                    v.ParentEdge = e;
                    v.HeapKey = weight;
                    heap.Remove(v);
                    heap.Add(v);
                }
            }
        }

        private static void postorderTraversal(Graph graph, Vertex root)
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
                max = graph.Verteces.Length - 1;
                lim = 0;
                root.ScanIndex = 0;
            }

            // root has index max in _graph.Vertices.
            for (var i = min; i < max; i++)
            {
                var v = graph.Verteces[i];
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
                        graph.Verteces[lim] = curr;
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

        /*private static void classifyVretices2(Graph graph, Edge edge, Action<Vertex> tailCallback, Action<Vertex> headCallback)
        {
            int low, lim;
            if (edge.Src.Lim > edge.Dst.Lim)
            {
                low = edge.Dst.Low;
                lim = edge.Dst.Lim;
            }
            else if (edge.Src.Lim < edge.Dst.Lim)
            {
                low = edge.Src.Low;
                lim = edge.Src.Lim;
            }
            else
                throw new Exception("Alg. error.");

            foreach (var testVertex in graph.Verteces)
            {
                var isHead = low <= testVertex.Lim && testVertex.Lim <= lim;
                if (isHead)
                    headCallback(testVertex);
                else
                    tailCallback(testVertex);
            }
        }

        private static void Dump(Graph graph, Edge breakingEdge)
        {
            var dg = new DotGraph();

            var headV = new List<Vertex>();
            var tailV = new List<Vertex>();

            classifyVretices2(graph, breakingEdge, tailV.Add, headV.Add);

            var tailDg = new DotGraph
            {
                Id = "cluster_0",
                Label = "Tail component"
            };
            var headDg = new DotGraph
            {
                Id = "cluster_1",
                Label = "Head component"
            };

            dg.SubGraphs.Add(tailDg);
            dg.SubGraphs.Add(headDg);
            foreach (var v in tailV)
            {
                var dv = new DotVertex
                {
                    Id = v.Id,
                    Label = string.Format("{0} ({1};{2})", v.Id, v.Low, v.Lim)
                };
                tailDg.Verteces.Add(dv);
            }

            foreach (var v in headV)
            {
                var dv = new DotVertex
                {
                    Id = v.Id,
                    Label = string.Format("{0} ({1};{2})", v.Id, v.Low, v.Lim),
                    Color = DotColors.blue
                };
                headDg.Verteces.Add(dv);
            }

            foreach (var edge in graph.Edges)
            {
                var de = new DotEdge();
                if (edge.Src.Lim > edge.Dst.Lim)
                {
                    de.Src = edge.Src.Id;
                    de.Dst = edge.Dst.Id;
                    de.Dir = DotEdgeDirection.forward;
                }
                else
                {
                    de.Src = edge.Dst.Id;
                    de.Dst = edge.Src.Id;
                    de.Dir = DotEdgeDirection.back;
                }

                if (edge.IsTree)
                {
                    de.PenWidth = edge == breakingEdge ? 3 : 1;
                    de.Label = string.Format("c={0}/w={1}", edge.CutValue, edge.Weight);
                }
                else
                {
                    de.Style = DotEdgeStyle.dotted;
                    de.Weight = 0;
                    de.Constraint = false;
                    de.Label = "w=" + edge.Weight;
                }
                dg.Edges.Add(de);
            }
            var asm = Assembly.GetExecutingAssembly();
            var directoryName = Path.GetDirectoryName(asm.Location);
            if (directoryName == null)
                return;
            var filePath = Path.Combine(directoryName, "dump.dot");
            using (var sw = File.CreateText(filePath))
                dg.Serialize(sw);
        }

        private static void Dump(Graph graph, string fileName = "dump.dot")
        {
            var dg = new DotGraph();

            foreach (var v in graph.Verteces)
            {
                var dv = new DotVertex
                {
                    Id = v.Id,
                    Label = string.Format("{0} ({1};{2})", v.Id, v.Low, v.Lim)
                };
                dg.Verteces.Add(dv);
            }

            foreach (var edge in graph.Edges)
            {
                var de = new DotEdge();
                if (edge.Src.Lim > edge.Dst.Lim)
                {
                    de.Src = edge.Src.Id;
                    de.Dst = edge.Dst.Id;
                    de.Dir = DotEdgeDirection.forward;
                }
                else
                {
                    de.Src = edge.Dst.Id;
                    de.Dst = edge.Src.Id;
                    de.Dir = DotEdgeDirection.back;
                }

                if (edge.IsTree)
                {
                    de.Label = string.Format("c={0}/w={1}", edge.CutValue, edge.Weight);
                }
                else
                {
                    de.Style = DotEdgeStyle.dotted;
                    de.Weight = 0;
                    de.Constraint = false;
                    de.Label = "w=" + edge.Weight;
                }
                dg.Edges.Add(de);
            }
            var asm = Assembly.GetExecutingAssembly();
            var directoryName = Path.GetDirectoryName(asm.Location);
            if (directoryName == null)
                return;
            var filePath = Path.Combine(directoryName, fileName);
            using (var sw = File.CreateText(filePath))
                dg.Serialize(sw);
        }

        private static void Dump(Graph graph, Vertex root, string fileName = "dump.dot")
        {
            var dg = new DotGraph();

            foreach (var v in graph.Verteces)
            {
                var dv = new DotVertex
                {
                    Id = v.Id,
                    Label = string.Format("{0} ({1};{2})", v.Id, v.Low, v.Lim)
                };
                if (v == root)
                    dv.Color = DotColors.red;
                dg.Verteces.Add(dv);
            }

            foreach (var edge in graph.Edges)
            {
                var de = new DotEdge();
                if (edge.Src.Lim > edge.Dst.Lim)
                {
                    de.Src = edge.Src.Id;
                    de.Dst = edge.Dst.Id;
                    de.Dir = DotEdgeDirection.forward;
                }
                else
                {
                    de.Src = edge.Dst.Id;
                    de.Dst = edge.Src.Id;
                    de.Dir = DotEdgeDirection.back;
                }

                if (edge.IsTree)
                {
                    de.Label = string.Format("c={0}/w={1}", edge.CutValue, edge.Weight);
                }
                else
                {
                    de.Style = DotEdgeStyle.dotted;
                    de.Weight = 0;
                    de.Constraint = false;
                    de.Label = "w=" + edge.Weight;
                }
                dg.Edges.Add(de);
            }
            var asm = Assembly.GetExecutingAssembly();
            var directoryName = Path.GetDirectoryName(asm.Location);
            if (directoryName == null)
                return;
            var filePath = Path.Combine(directoryName, fileName);
            using (var sw = File.CreateText(filePath))
                dg.Serialize(sw);
        }*/
    }
}